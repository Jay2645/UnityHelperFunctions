using Clickable.Text.Menu;
using System.Collections.Generic;
using UnityEngine;
public class InputController : MonoHelper
{
	public static Vector3 mousePos;
	private List<Collider2D> hits2D = new List<Collider2D>();
	private List<Collider> hits3D = new List<Collider>();
	private Camera guiCamera;
	private const SendMessageOptions HIT_OPTIONS = SendMessageOptions.DontRequireReceiver;

	public static InputController instance;

	#region Mobile
	private static int touchCount = 0;
	public static int moveDirection = 0;
	public static bool isLeftClicking = false;
	public static bool isRightClicking = false;
	public static bool isMiddleClicking = false;
	public static bool isJumping = false;
	private static float touchTime = 0.0f;
	private static float timeBetweenTouches = 0.0f;
	private const float MIN_TOUCH_TIME = 0.25f;
	private const float LONG_PRESS_TIME = 1.0f;
	private const float SCREEN_PADDING = 0.5f;
	#endregion

	void Awake()
	{
		if (Globals.inputController != null && Globals.inputController != this)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		GameObject cameraGO = GameObject.FindGameObjectWithTag("GUI Camera");
		if (cameraGO != null)
		{
			guiCamera = cameraGO.GetComponent<Camera>();
		}
	}

	// Update is called once per frame
	void LateUpdate()
	{
		ResetValues();
		CheckForKeyboardInput();
		CheckForMouseInput();
	}

	private void ResetValues()
	{
		if (timeBetweenTouches > 0)
		{
			touchCount = 0;
			timeBetweenTouches -= Time.fixedDeltaTime;
		}
		else
		{
			touchCount = Input.touchCount;
		}
		if (touchCount == 0 && touchTime > 0)
		{
			touchTime = 0.0f;
		}
		moveDirection = 0;
		isLeftClicking = false;
		isRightClicking = false;
		isMiddleClicking = false;
		mousePos = GetMousePosition();
	}

	private Vector3 GetMousePosition()
	{
		Vector3 mousePosition = Vector3.zero;
		if (touchCount > 0)
		{
			Touch t = Input.GetTouch(0);
			mousePosition = t.position;
		}
		else
		{
			mousePosition = Input.mousePosition;
		}
		return mousePosition;
	}

	private void CheckForKeyboardInput()
	{
		if (Globals.gameIsOver)
		{
			if (Input.anyKeyDown || Input.touchCount > 0)
			{
				Globals.CleanUpObjects();
				CameraFade.StartAlphaFade(Color.black, false, true, GlobalConsts.SCENE_TRANSITION_TIME, 0.0f, () =>
				{
					Application.LoadLevel(0);
				});
			}
			return;
		}
		if (touchCount > 0)
		{
			isLeftClicking = true;
			float width = Screen.width / 2;
			float paddedWidth = width * SCREEN_PADDING;
			float maxInputPosition = width + paddedWidth;
			if (mousePos.x > maxInputPosition)
			{
				moveDirection = 1;
			}
			else
			{
				float minInputPosition = width - paddedWidth;
				if (mousePos.x < minInputPosition)
				{
					moveDirection = -1;
				}
			}
		}
	}

	private void CheckForMouseInput()
	{
		List<Collider2D> current2DHits = new List<Collider2D>();
		List<Collider> current3DHits = new List<Collider>();
		GameObject[] camGos = GameObject.FindGameObjectsWithTag("MainCamera");
		bool isPaused = Globals.GameIsPaused();
		foreach (GameObject go in camGos)
		{
			Camera cam = go.GetComponent<Camera>();
			if (!go.activeInHierarchy || cam == null)
			{
				continue;
			}
			current2DHits.AddRange(CheckHits2D(cam));
			current3DHits.AddRange(CheckHits(cam));
		}
		if (guiCamera != null)
		{
			current2DHits.AddRange(CheckHits2D(guiCamera));
			current3DHits.AddRange(CheckHits(guiCamera));
		}
		// Clean up things the mouse isn't pointing at anymore.
		// 2D
		Collider2D[] all2DCollisions = hits2D.ToArray();
		foreach (Collider2D col in all2DCollisions)
		{
			if (current2DHits.Contains(col) || isPaused && col.GetComponent<MenuResponse>() == null)
				continue;
			hits2D.Remove(col);
			if (col == null)
			{
				continue;
			}
			col.gameObject.SendMessage("_MouseExit", HIT_OPTIONS);
		}
		foreach (Collider2D col in current2DHits)
		{
			if (col == null)
			{
				continue;
			}
			col.gameObject.SendMessage("_MouseOver", HIT_OPTIONS);
		}

		// 3D
		Collider[] all3DCollisions = hits3D.ToArray();
		foreach (Collider col in all3DCollisions)
		{
			if (current3DHits.Contains(col) || isPaused && col.GetComponent<MenuResponse>() == null)
				continue;
			hits3D.Remove(col);
			if (col == null)
			{
				continue;
			}
			col.gameObject.SendMessage("_MouseExit", HIT_OPTIONS);
		}
		foreach (Collider col in current3DHits)
		{
			if (col == null)
			{
				continue;
			}
			col.gameObject.SendMessage("_MouseOver", HIT_OPTIONS);
		}


		isLeftClicking = Input.GetMouseButton(0);
		isRightClicking = Input.GetMouseButton(1);
		isMiddleClicking = Input.GetMouseButton(2);
	}

	private List<Collider2D> CheckHits2D(Camera camera)
	{
		Ray mouseRay = camera.ScreenPointToRay(mousePos);
		List<Collider2D> currentHits = new List<Collider2D>();
		RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
		if (hit.collider != null)
		{
			Collider2D hitCollider2D = hit.collider;
			GameObject hitGO = hitCollider2D.gameObject;
			if (!hits2D.Contains(hitCollider2D))
			{
				hits2D.Add(hitCollider2D);
				hitGO.SendMessage("_MouseEnter", HIT_OPTIONS);
			}
			currentHits.Add(hitCollider2D);
			if (touchCount > 0 && moveDirection == 0)
			{
				if (touchTime < LONG_PRESS_TIME)
				{
					if (timeBetweenTouches <= 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
					{
						LeftClick(hitGO);
						timeBetweenTouches = MIN_TOUCH_TIME;
					}
				}
				else
				{
					RightClick(hitGO);
					timeBetweenTouches = MIN_TOUCH_TIME;
				}
				touchTime += Time.fixedDeltaTime;
			}
			else
			{
				if (Input.GetMouseButtonUp(0))
				{
					LeftClick(hitGO);
				}
				else if (Input.GetMouseButtonUp(1))
				{
					RightClick(hitGO);
				}
				else if (Input.GetMouseButtonUp(2))
				{
					MiddleClick(hitGO);
				}
			}
		}
		return currentHits;
	}

	private List<Collider> CheckHits(Camera camera)
	{
		Ray mouseRay = camera.ScreenPointToRay(mousePos);
		RaycastHit hit;
		List<Collider> currentHits = new List<Collider>();
		if (Physics.Raycast(mouseRay, out hit))
		{
			Collider hitCollider = hit.collider;
			GameObject hitGO = hitCollider.gameObject;
			if (!hits3D.Contains(hitCollider))
			{
				hits3D.Add(hitCollider);
				hitGO.SendMessage("_MouseEnter", HIT_OPTIONS);
			}
			currentHits.Add(hitCollider);
			if (touchCount > 0 && moveDirection == 0)
			{
				if (touchTime < LONG_PRESS_TIME)
				{
					if (timeBetweenTouches <= 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
					{
						LeftClick(hitGO);
						timeBetweenTouches = MIN_TOUCH_TIME;
					}
				}
				else
				{
					RightClick(hitGO);
					timeBetweenTouches = MIN_TOUCH_TIME;
				}
				touchTime += Time.fixedDeltaTime;
			}
			else
			{
				if (Input.GetMouseButtonUp(0))
				{
					LeftClick(hitGO);
				}
				else if (Input.GetMouseButtonUp(1))
				{
					RightClick(hitGO);
				}
				else if (Input.GetMouseButtonUp(2))
				{
					MiddleClick(hitGO);
				}
			}
		}
		return currentHits;
	}

	private void LeftClick(GameObject hit)
	{
		hit.SendMessage("_LeftClicked", HIT_OPTIONS);
	}
	private void RightClick(GameObject hit)
	{
		hit.SendMessage("_RightClicked", HIT_OPTIONS);
	}
	private void MiddleClick(GameObject hit)
	{
		hit.SendMessage("_MiddleClicked", HIT_OPTIONS);
	}

	public override JSONSystem.JSONClass ToJSON()
	{
		return null;
	}

	public override void FromJSON(JSONSystem.JSONClass json)
	{
	}
}
