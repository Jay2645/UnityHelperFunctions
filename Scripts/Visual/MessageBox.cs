using UnityEngine;
using ObserverSystem;
using Clickable.Text;
namespace LuaSystem
{
	public class MessageBoxLuaBinding : LuaBinding
	{
		public MessageBoxLuaBinding()
		{
			SetBinding("messagesystem");
		}

		public void ChangeBoxPrompt(string prompt)
		{
			ChangeBoxPrompt(prompt, "", true);
		}

		public void ChangeBoxPrompt(string prompt, string id)
		{
			ChangeBoxPrompt(prompt, id, true);
		}

		public void ChangeBoxPrompt(string prompt, string id, bool open)
		{
			MessageBox.ChangeBoxPrompt(new LocalizedString(id, prompt), open);
		}

		public void AddResponse(string prompt)
		{
			AddResponse(prompt, "");
		}

		public void AddResponse(string prompt, string id)
		{
			MessageBox.AddResponse(new LocalizedString(id, prompt));
		}

		public void ClearResponses()
		{
			MessageBox.ClearResponses();
		}

		public void Open()
		{
			MessageBox.DisplayBox(true);
		}

		public void Close()
		{
			MessageBox.DisplayBox(false);
		}
	}
}

public static class MessageBox
{
	public static LocalizedString boxPrompt = null;
	private static GameObject messageBox = null;
	private static DisplayText messagePrompt = null;
	private static System.Collections.Generic.List<LocalizedString> responseStrings = new System.Collections.Generic.List<LocalizedString>();
	private static ClickableText[] responses = null;
	private static LuaSystem.MessageBoxLuaBinding binding = null;
	private static Subject subject = null;
	public static bool isOpen = false;
	private static bool defaultResponses = true;

	public static void ChangeBoxPrompt(LocalizedString newPrompt)
	{
		ChangeBoxPrompt(newPrompt, false);
	}

	public static void InitLua()
	{
		if (subject == null)
		{
			subject = new Subject();
		}
		if (binding == null)
		{
			binding = new LuaSystem.MessageBoxLuaBinding();
			subject.AddObserver(binding);
		}
	}

	public static void ChangeBoxPrompt(LocalizedString newPrompt, bool open)
	{
		boxPrompt = newPrompt;
		CheckPrompt();
		messagePrompt.SetTextString(boxPrompt);
		if (responses == null)
		{
			AddResponse(new LocalizedString(GlobalConsts.ID_OKAY, "Okay"));
		}
		InitLua();
		DisplayBox(open);
	}

	public static ClickableText AddResponse(LocalizedString text)
	{
		return AddResponse(text, typeof(ClickableText));
	}

	public static ClickableText AddResponse(LocalizedString text, System.Type type)
	{
		if (!typeof(ClickableText).IsAssignableFrom(type))
		{
			return null;
		}
		if (messageBox != null && !messageBox.activeSelf)
		{
			messageBox.SetActive(true);
		}
		ClickableText output = null;
		if (responses != null)
		{
			foreach (ClickableText cText in responses)
			{
				MonoBehaviour.Destroy(cText.gameObject);
			}
		}
		if (defaultResponses)
		{
			ClearResponses();
		}
		responseStrings.Add(text);
		responses = new ClickableText[responseStrings.Count];
		// Determine the number of columns
		int responseCount = responses.Length;
		int responseColumns = 1; // Start at 1
		if (responseCount % 2 == 0) // If it's divisible by 2, we should have either 2 or 4 columns.
		{
			if (responseCount <= 4)
			{
				responseColumns = 2;
			}
			else
			{
				responseColumns = 4;
			}
		}
		else if (responseCount % 3 == 0) // If it's divisible by 3, we should have 3 columns.
		{
			responseColumns = 3;
		}

		// Figure out how many responses will be in each column
		int responsesPerColumn = Mathf.CeilToInt(responseCount / responseColumns);

		DisplayText lastText = messagePrompt;

		// Determine box X positions
		float[] responseXs = new float[responseColumns];
		float boxWidth = GetBackgroundWidth();
		float minXPos = boxWidth / -2.0f;
		float responseMaxWidth = boxWidth / responseColumns;
		for (int i = 0; i < responseColumns; i++)
		{
			float responseX = minXPos;
			responseX += (responseMaxWidth * i) + (responseMaxWidth / 2.0f);
			responseXs[i] = responseX;
		}
		responseMaxWidth -= GlobalConsts.RESPONSE_SPACING;

		// Determine box Y positions
		float[] responseYs = new float[responsesPerColumn];
		float boxHeight = GetBackgroundHeight();
		float minYPos = boxHeight / -2.0f;
		float responseMaxHeight = boxHeight / responsesPerColumn;
		for (int i = 0; i < responsesPerColumn; i++)
		{
			float responseY = minYPos;
			responseY += (responseMaxHeight * i) + (responseMaxHeight / 2.0f);
			responseYs[i] = responseY;
		}
		responseMaxHeight -= GlobalConsts.RESPONSE_SPACING;

		int count = 0;
		for (int x = 0; x < responseColumns; x++)
		{
			Vector3 position = new Vector3(responseXs[x], 0.0f, -0.1f);
			bool allResponsesDone = false;
			for (int y = 0; y < responsesPerColumn; y++)
			{
				if (count >= responses.Length)
				{
					allResponsesDone = true;
					break;
				}
				position.y = responseYs[y];
				// Load GameObject
				GameObject responseGO = MonoHelper.Load(lastText.transform.parent, position, "Prefabs/Response Text") as GameObject;
				ClickableText response = responseGO.GetComponentInChildren(type) as ClickableText;
				// Set maximum bounds
				response.SetMaxWidth(responseMaxWidth);
				response.SetMaxHeight(responseMaxHeight);
				output = response;

				// Set visuals
				response.SetTextString(responseStrings[count]);
				response.SetDisplayParent(messageBox);

				// Make the instantiated object clickable if it isn't already
				if (responseGO.GetComponent<BoxCollider2D>() == null)
				{
					responseGO.AddComponent<BoxCollider2D>();
				}
				// Reset lastText to the most recent text.
				lastText = response.GetDisplayText();
				responses[count] = response;
				// Keep it alive on scene change
				Globals.DontDestroy(responseGO);
				count++;
			}
			if (allResponsesDone)
			{
				break;
			}
		}
		if (text.ID != "OK")
		{
			defaultResponses = false;
		}
		return output;
	}

	private static void CheckPrompt()
	{
		if (messageBox == null)
		{
			if (Globals.player == null)
			{
				return;
			}
			GameObject guiCam = GameObject.FindGameObjectWithTag("GUI Camera");
			if (guiCam == null)
			{
				Vector3 boxPosition = new Vector3(0.2350187f, -19.13935f, -0.3204548f);
				messageBox = MonoHelper.Load(boxPosition, "Prefabs/Menus/Event") as GameObject;
			}
			else
			{
				messageBox = MonoHelper.Load(guiCam.transform, new Vector3(0, 0, 9), "Prefabs/Menus/Event") as GameObject;
			}

			messagePrompt = messageBox.GetComponentInChildren<DisplayText>();
		}
		else if (messagePrompt == null)
		{
			messagePrompt = messageBox.GetComponentInChildren<DisplayText>();
		}
	}

	public static void ChooseResponse(ClickableText response, string id)
	{
		DisplayBox(false);
		subject.Notify(response, id);
		ClearResponses();
	}

	public static void DisplayBox(bool open)
	{
		CheckPrompt();
		messageBox.SetActive(open);
		Globals.gameIsPaused = open;
		isOpen = open;
	}

	public static void ClearResponses()
	{
		responseStrings.Clear();
		if (responses == null)
		{
			return;
		}
		foreach (ClickableText response in responses)
		{
			MonoBehaviour.Destroy(response.gameObject);
		}
		responses = null;
		defaultResponses = true;
	}

	private static Vector2[] GetBackgroundBounds()
	{
		if (messageBox == null || messageBox.renderer == null)
		{
			return null;
		}
		Vector2[] bounds = new Vector2[4];
		Bounds b = messageBox.renderer.bounds;
		Vector2 center = b.center;
		Vector2 extents = b.extents;
		bounds[0] = new Vector2(center.x - extents.x, center.y + extents.y); // Top left
		bounds[1] = center + extents; // Top right
		bounds[2] = center - extents; // Bottom left
		bounds[3] = new Vector2(center.x + extents.x, center.y - extents.y); // Bottom right
		return bounds;
	}

	private static float GetBackgroundWidth()
	{
		if (messageBox == null || messageBox.renderer == null)
		{
			return 0.0f;
		}
		Bounds b = messageBox.renderer.bounds;
		Vector2 extents = b.extents;
		return extents.x * 2.0f;
	}

	private static float GetBackgroundHeight()
	{
		if (messageBox == null || messageBox.renderer == null)
		{
			return 0.0f;
		}
		Bounds b = messageBox.renderer.bounds;
		Vector2 extents = b.extents;
		return extents.y * 2.0f;
	}
}