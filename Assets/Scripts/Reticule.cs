using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticule : MonoBehaviour
{
    public Pointer m_Pointer;
    public SpriteRenderer m_circleRenderer;

    public Sprite m_OpenSprite;
    public Sprite m_CloseSprite;

    private Camera m_Camera = null;

    private void Awake()
    {
        m_Pointer.OnPointerUpdate += UpdateSprite;
        PlayerEvents.onTouchpadDown += ProcessTouchPadDown;
        PlayerEvents.onTouchpadUp += ProcessTouchPadUp;

        m_Camera = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(m_Camera.gameObject.transform);
    }

    private void OnDestroy()
    {
        m_Pointer.OnPointerUpdate -= UpdateSprite;
        PlayerEvents.onTouchpadDown -= ProcessTouchPadDown;
        PlayerEvents.onTouchpadUp -= ProcessTouchPadUp;
    }

    private void UpdateSprite(Vector3 point, GameObject hitObject)
    {
        transform.position = point;
        m_circleRenderer.sprite = hitObject ? m_CloseSprite : m_OpenSprite;
    }

    private void ProcessTouchPadDown()
    {
        m_circleRenderer.color = m_Pointer.m_PointerPressedColor;
    }

    private void ProcessTouchPadUp()
    {
        m_circleRenderer.color = m_Pointer.m_PointerStandardColor;
    }
}
