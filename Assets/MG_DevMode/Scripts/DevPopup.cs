using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
#if UNITY_EDITOR && UNITY_2021_1_OR_NEWER
using Screen = UnityEngine.Device.Screen; // To support Device Simulator on Unity 2021.1+
#endif

// Manager class for the debug popup
namespace MysticDev
{
	public class DevPopup : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		private RectTransform popupTransform;

		// Dimensions of the popup divided by 2
		private Vector2 halfSize;
		
		[SerializeField]
		private Vector2 miniSize = new Vector2( 80f, 80f);
		[SerializeField]
		private Vector2 fullSize = new Vector2(170f, 195f);

		// Background image that will change color to indicate an alert
		private Image backgroundImage;

		// Canvas group to modify visibility of the popup
		private CanvasGroup canvasGroup;
		
		[SerializeField]
		private DevManager devManager;

		// Number of new debug entries since the log window has been closed
		private int newInfoCount = 0, newWarningCount = 0, newErrorCount = 0;

		private Color normalColor;

		private bool isPopupBeingDragged = false;
		private Vector2 normalizedPosition;

		// Coroutines for simple code-based animations
		private IEnumerator moveToPosCoroutine = null;

		[Header("Params")]
		[SerializeField]
		private GameObject miniGO;
		[SerializeField]
		private GameObject fullGO;

		public Text debugLogText;
		public Text adsText;
		
		
		public bool IsVisible { get; private set; }

		private void Awake()
		{
			Debug.Log("Awake: Dev ");
			popupTransform = (RectTransform) transform;
			backgroundImage = GetComponent<Image>();
			canvasGroup = GetComponent<CanvasGroup>();

			normalColor = backgroundImage.color;

			
		}

		private void OnEnable()
		{
			// Show mini content and hide full content
			miniGO.SetActive( true );
			fullGO.SetActive( false );

			popupTransform.sizeDelta = miniSize;
			
			halfSize = popupTransform.sizeDelta * 0.5f;

			Vector2 pos = popupTransform.anchoredPosition;
			if( pos.x != 0f || pos.y != 0f )
				normalizedPosition = pos.normalized; // Respect the initial popup position set in the prefab
			else
				normalizedPosition = new Vector2( 0.5f, 0f ); // Right edge by default
		}


		// A simple smooth movement animation
		private IEnumerator MoveToPosAnimation( Vector2 targetPos )
		{
			float modifier = 0f;
			Vector2 initialPos = popupTransform.anchoredPosition;

			while( modifier < 1f )
			{
				modifier += 4f * Time.unscaledDeltaTime;
				popupTransform.anchoredPosition = Vector2.Lerp( initialPos, targetPos, modifier );

				yield return null;
			}
		}

		// Popup is clicked
		public void OnPointerClick( PointerEventData data )
		{
			// Hide the popup and show the log window
			Debug.Log("Click Dev Popup");
			if( !isPopupBeingDragged )
				ShowFullContent();
		}

		
		// Show Full Content
		public void ShowFullContent()
		{
			miniGO.SetActive( false );
			fullGO.SetActive( true );
			popupTransform.sizeDelta = fullSize;
			halfSize = popupTransform.sizeDelta * 0.5f;
			UpdatePosition( true );
		}

		public void ShowMiniContent()
		{
			miniGO.SetActive( true );
			fullGO.SetActive( false );
			popupTransform.sizeDelta = miniSize;
			halfSize = popupTransform.sizeDelta * 0.5f;
			UpdatePosition( true );
		}


		#region Touch Move

		public void OnBeginDrag( PointerEventData data )
		{
			isPopupBeingDragged = true;

			// If a smooth movement animation is in progress, cancel it
			if( moveToPosCoroutine != null )
			{
				StopCoroutine( moveToPosCoroutine );
				moveToPosCoroutine = null;
			}
		}

		// Reposition the popup
		public void OnDrag( PointerEventData data )
		{
			Vector2 localPoint;
			if( RectTransformUtility.ScreenPointToLocalPointInRectangle( devManager.canvasTR, data.position, data.pressEventCamera, out localPoint ) )
				popupTransform.anchoredPosition = localPoint;
		}

		// Smoothly translate the popup to the nearest edge
		public void OnEndDrag( PointerEventData data )
		{
			isPopupBeingDragged = false;
			UpdatePosition( false );
		}

		// There are 2 different spaces used in these calculations:
		// RectTransform space: raw anchoredPosition of the popup that's in range [-canvasSize/2, canvasSize/2]
		// Safe area space: Screen.safeArea space that's in range [safeAreaBottomLeft, safeAreaTopRight] where these corner positions
		//                  are all positive (calculated from bottom left corner of the screen instead of the center of the screen)
		public void UpdatePosition( bool immediately )
		{
			Vector2 canvasRawSize = devManager.canvasTR.rect.size;

			// Calculate safe area bounds
			float canvasWidth = canvasRawSize.x;
			float canvasHeight = canvasRawSize.y;

			float canvasBottomLeftX = 0f;
			float canvasBottomLeftY = 0f;

			if( devManager.popupAvoidsScreenCutout )
			{
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
				Rect safeArea = Screen.safeArea;

				int screenWidth = Screen.width;
				int screenHeight = Screen.height;

				canvasWidth *= safeArea.width / screenWidth;
				canvasHeight *= safeArea.height / screenHeight;

				canvasBottomLeftX = canvasRawSize.x * ( safeArea.x / screenWidth );
				canvasBottomLeftY = canvasRawSize.y * ( safeArea.y / screenHeight );
#endif
			}

			// Calculate safe area position of the popup
			// normalizedPosition allows us to glue the popup to a specific edge of the screen. It becomes useful when
			// the popup is at the right edge and we switch from portrait screen orientation to landscape screen orientation.
			// Without normalizedPosition, popup could jump to bottom or top edges instead of staying at the right edge
			Vector2 pos = canvasRawSize * 0.5f + ( immediately ? new Vector2( normalizedPosition.x * canvasWidth, normalizedPosition.y * canvasHeight ) : ( popupTransform.anchoredPosition - new Vector2( canvasBottomLeftX, canvasBottomLeftY ) ) );

			// Find distances to all four edges of the safe area
			float distToLeft = pos.x;
			float distToRight = canvasWidth - distToLeft;

			float distToBottom = pos.y;
			float distToTop = canvasHeight - distToBottom;

			float horDistance = Mathf.Min( distToLeft, distToRight );
			float vertDistance = Mathf.Min( distToBottom, distToTop );

			// Find the nearest edge's safe area coordinates
			if( horDistance < vertDistance )
			{
				if( distToLeft < distToRight )
					pos = new Vector2( halfSize.x, pos.y );
				else
					pos = new Vector2( canvasWidth - halfSize.x, pos.y );

				pos.y = Mathf.Clamp( pos.y, halfSize.y, canvasHeight - halfSize.y );
			}
			else
			{
				if( distToBottom < distToTop )
					pos = new Vector2( pos.x, halfSize.y );
				else
					pos = new Vector2( pos.x, canvasHeight - halfSize.y );

				pos.x = Mathf.Clamp( pos.x, halfSize.x, canvasWidth - halfSize.x );
			}

			pos -= canvasRawSize * 0.5f;

			normalizedPosition.Set( pos.x / canvasWidth, pos.y / canvasHeight );

			// Safe area's bottom left coordinates are added to pos only after normalizedPosition's value
			// is set because normalizedPosition is in range [-canvasWidth / 2, canvasWidth / 2]
			pos += new Vector2( canvasBottomLeftX, canvasBottomLeftY );

			// If another smooth movement animation is in progress, cancel it
			if( moveToPosCoroutine != null )
			{
				StopCoroutine( moveToPosCoroutine );
				moveToPosCoroutine = null;
			}

			if( immediately )
				popupTransform.anchoredPosition = pos;
			else
			{
				// Smoothly translate the popup to the specified position
				moveToPosCoroutine = MoveToPosAnimation( pos );
				StartCoroutine( moveToPosCoroutine );
			}
		}
		
		#endregion
	}
}