using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public enum HandType { Left, Right }
public class Hand : MonoBehaviour
{
    public HandType type = HandType.Left;

    public bool isHidden { get; private set; } = false;

    public InputAction trackedAction = null;

    bool m_isCurrentlyTracked = false;

    List<Renderer> m_currentRenderers = new List<Renderer>();

    Collider[] m_colliders = null;
    public bool isCollisionEnabled { get; private set; } = false;

    public XRBaseInteractor interactor = null;

    private void Awake()
    {
        if(interactor == null)
        {
            interactor = GetComponentInParent<XRBaseInteractor>();
        }
    }

    private void OnEnable()
    {
        interactor.onSelectEntered.AddListener(OnGrab);
        interactor.onSelectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        interactor.onSelectEntered.RemoveListener(OnGrab);
        interactor.onSelectExited.RemoveListener(OnRelease);
    }
    // Start is called before the first frame update
    void Start()
    {
        m_colliders = GetComponentsInChildren<Collider>().Where(childCollider => !childCollider.isTrigger).ToArray();
        trackedAction.Enable();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        float isTracked = trackedAction.ReadValue<float>();
        if(isTracked == 1.0f && !m_isCurrentlyTracked)
        {
            m_isCurrentlyTracked = true;
            Show();
        }
        else if (isTracked == 0.0 && m_isCurrentlyTracked)
        {
            m_isCurrentlyTracked = false;
            Hide();        }

    }

    public void Show()
    {
        foreach (Renderer renderer  in m_currentRenderers)
        {
            renderer.enabled = true;

        }
        isHidden = false;
        EnableCollisions(true);
    }

    public void Hide()
    {
        m_currentRenderers.Clear();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            renderer.enabled = false;
            m_currentRenderers.Add(renderer);
        }
        isHidden = true;
        EnableCollisions(false);
    }

    public void EnableCollisions(bool enabled)
    {
        if (isCollisionEnabled == enabled) return;

        isCollisionEnabled = enabled;
        {
            foreach(Collider collider in m_colliders)
            {
                collider.enabled = isCollisionEnabled;
            }
        }
    }

    void OnGrab(XRBaseInteractable grabbedObject)
    {
        HandControl ctrl = grabbedObject.GetComponent<HandControl>();
        if(ctrl != null)
        {
            if (ctrl.hideHand)
            {
                Hide();
            }
        }
    }

    void OnRelease(XRBaseInteractable releasedObject)
    {
        HandControl ctrl = releasedObject.GetComponent<HandControl>();
        if(ctrl != null)
        {
            if(ctrl.hideHand)
            {
                Show();
            }
        }
        
    }

    public void InteractionCleanup()
    {
        XRDirectInteractor directInteractor = interactor as XRDirectInteractor;
        if(directInteractor != null)
        {
            List<XRBaseInteractable> validTargets = new List<XRBaseInteractable>();
            interactor.GetValidTargets(validTargets);
            interactor.interactionManager.ClearInteractorSelection(interactor);
            interactor.interactionManager.ClearInteractorHover(interactor, validTargets);

            foreach (var target in validTargets)
            {
                if(target.gameObject.scene != interactor.gameObject.scene)
                {
                    target.transform.position = new Vector3(100, 100, 100);
                }
            }
        }
    }
}



