using UnityEngine;
namespace HelperFunctions
{
	namespace Clickable
	{
		namespace Text
		{
			namespace Menu
			{
				/// <summary>
				/// Represents an option on an in-game menu.
				/// </summary>
				[RequireComponent(typeof(SpriteRenderer))]
				public class MenuResponse : ClickableObject
				{

					#region Variables
					/// <summary>
					/// The sprite which the user clicks on.
					/// </summary>
					protected SpriteRenderer loadSprite;
					protected float timeSinceStartup = 0.0f;
					public ParticleEmitter[] emitters;
					#endregion

					#region ClickableObject Methods
					protected override void Start()
					{
						base.Start();
						useBounds = false;
						loadSprite = gameObject.GetComponent<SpriteRenderer>();
						nonHoverColor = Color.white;
						currentColor = Color.white;
						timeSinceStartup = Time.realtimeSinceStartup;
					}

					protected override void LateUpdate()
					{
						float deltaTime = Time.realtimeSinceStartup - timeSinceStartup;
						if (mouseOver)
						{
							currentColor = Color.Lerp(currentColor, hoverColor, deltaTime * 5);
						}
						else
						{
							currentColor = Color.Lerp(currentColor, nonHoverColor, deltaTime * 5);
						}
						loadSprite.color = currentColor;
						if (emitters != null)
						{
							foreach (ParticleEmitter emit in emitters)
							{
								emit.emit = mouseOver;
							}
						}
						timeSinceStartup = Time.realtimeSinceStartup;
					}
					#endregion
				}
			}
		}
	}
}