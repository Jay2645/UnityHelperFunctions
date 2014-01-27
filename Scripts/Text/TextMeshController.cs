using UnityEngine;
namespace Clickable
{
	namespace Text
	{
		[RequireComponent(typeof(TextMesh))]
		[ExecuteInEditMode]
		public class TextMeshController : MonoBehaviour
		{
			#region public members.
			public float m_maxWidth = 0.0f;
			public bool m_disableWhilePlaying = true;
			public string text = "";
			#endregion

			#region private members.
			private TextMesh m_textMesh = null;
			private float m_lastMaxWidth = 0.0f;
			private string m_wrappedText = "";
			private string lastText = "";
			#endregion



			#region getters/setters.
			public float TextWidth
			{
				get
				{
					return m_textMesh.renderer.bounds.extents.x * 2.0f;
				}
			}

			public float TextHeight
			{
				get
				{
					return m_textMesh.renderer.bounds.extents.y * 2.0f;
				}
			}

			public float MaxWidth
			{
				get
				{
					return m_maxWidth;
				}
			}
			#endregion



			#region monobehavior methods.
			public void Awake()
			{
				m_textMesh = GetComponent<TextMesh>();
				bool shouldDisable = m_disableWhilePlaying && !(Application.isEditor && !Application.isPlaying);
				this.enabled = !shouldDisable;
				if (!m_disableWhilePlaying && m_maxWidth > 0.0f)
				{
					UpdateText();
				}
			}



			public void OnDrawGizmos()
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawRay(GetInitPosition() + transform.up * 0.05f, transform.right * TextWidth);
				Gizmos.color = Color.red;
				Gizmos.DrawRay(GetInitPosition(), transform.right * MaxWidth);
			}

			public void Update()
			{
				if (m_textMesh.text != m_wrappedText || MaxWidth != m_lastMaxWidth || text != lastText)
				{
					UpdateText();
					lastText = text;
				}
			}
			#endregion



			#region private methods.

			private float GetSpaceWidth()
			{
				m_textMesh.text = "x x";

				float totalWidth = m_textMesh.renderer.bounds.extents.x * 2.0f;

				totalWidth -= GetWordWidth("xx");

				return totalWidth;

			}



			private float GetWordWidth(string word)
			{

				m_textMesh.text = word;

				return m_textMesh.renderer.bounds.extents.x * 2.0f;

			}



			private Vector3 GetInitPosition()
			{

				// Find X.

				float x = 0.0f;

				switch (m_textMesh.anchor)
				{

					case TextAnchor.LowerLeft:

					case TextAnchor.MiddleLeft:

					case TextAnchor.UpperLeft:

						x = transform.position.x;

						break;

					case TextAnchor.LowerRight:

					case TextAnchor.MiddleRight:

					case TextAnchor.UpperRight:

						x = transform.position.x - TextWidth;

						break;

					case TextAnchor.LowerCenter:

					case TextAnchor.MiddleCenter:

					case TextAnchor.UpperCenter:

						x = transform.position.x - TextWidth / 2.0f;

						break;

				}



				// Find Y.

				float y = 0.0f;

				switch (m_textMesh.anchor)
				{

					case TextAnchor.LowerLeft:

					case TextAnchor.LowerRight:

					case TextAnchor.LowerCenter:

						y = transform.position.y - TextHeight / 2.0f;

						break;

					case TextAnchor.MiddleRight:

					case TextAnchor.MiddleLeft:

					case TextAnchor.MiddleCenter:

						y = transform.position.y + TextHeight / 2.0f;

						break;

					case TextAnchor.UpperLeft:

					case TextAnchor.UpperRight:

					case TextAnchor.UpperCenter:

						y = transform.position.y;

						break;

				}

				return new Vector3(x, y, transform.position.z);

			}

			#endregion

			#region public methods
			public void UpdateText()
			{
				Quaternion rot = transform.rotation;
				transform.rotation = Quaternion.identity;
				float widthSum = 0.0f;
				m_wrappedText = "";
				string rawText = text;
				if (rawText == "")
				{
					rawText = m_textMesh.text;
				}
				string[] lines = rawText.Split('\n');
				for (int s = 0; s < lines.Length; s++)
				{
					string line = "";
					char[] separators = new char[] { ' ', '\n', '\t' };
					string[] words = lines[s].Split(separators);
					for (int i = 0; i < words.Length; i++)
					{
						string word = words[i].Trim();
						if (word == "")
						{
							continue;
						}
						if (i == 0)
						{
							line = word;
							widthSum = GetWordWidth(word);
						}
						else
						{
							// Concatenate the new word if there is still enough space.
							if (widthSum + GetWordWidth(word) <= MaxWidth)
							{
								if (widthSum > 0.0f)
								{
									line += ' ';
									widthSum += GetSpaceWidth();
								}
								line += word;
								widthSum += GetWordWidth(word);
							}
							// New line.
							else
							{
								if (m_wrappedText.Length > 0)
									m_wrappedText += '\n';
								m_wrappedText += line;
								line = word;
								widthSum = GetWordWidth(word);
							}
						}
					}
					if (widthSum > 0.0f)
					{
						if (m_wrappedText.Length > 0)
							m_wrappedText += '\n';
						m_wrappedText += line;
					}
				}
				m_textMesh.text = m_wrappedText;
				m_lastMaxWidth = MaxWidth;
				transform.rotation = rot;
			}
			#endregion

		}
	}
}