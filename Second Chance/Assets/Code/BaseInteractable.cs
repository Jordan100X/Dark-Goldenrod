using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ButtonType { OneTouch, Timeout, Hold, Continuous };

public class BaseInteractable : MonoBehaviour
{

    public List<BaseInteractable> actives;
    public List<BaseInteractable> recieves;
    public bool active;
    public float timeout;
    public ButtonType buttontype;

    public virtual void Activate()
    {
        if (CheckRecieves())
        {
            switch (buttontype)
            {
                case ButtonType.OneTouch:
                    active = true;
                    ActivateActives();
                    break;
                case ButtonType.Timeout:
                    StartCoroutine(TimeoutCoroutine());
                    break;
                case ButtonType.Continuous:
                case ButtonType.Hold:
                    StartCoroutine(ContinuousCoroutine());
                    break;
            }
        }
    }
    public virtual void UnActivate()
    {
        active = false;
        if (buttontype != ButtonType.Continuous)
            ActivateActives();
    }

    private bool CheckRecieves()
    {
        for (int i = 0; i < recieves.Count; i++)
        {
            if (!recieves[i].GetComponent<BaseInteractable>().active)
                return false;
        }
        return true;
    }
    private void ActivateActives()
    {
        for (int i = 0; i < actives.Count; i++)
        {
            recieves[i].GetComponent<BaseInteractable>().GetActivated();
        }
    }
    public virtual void GetActivated()
    {
        if (CheckRecieves())
            active = true;
        else
            active = false;
    }

    IEnumerator TimeoutCoroutine()
    {
        active = true;
        ActivateActives();
        yield return new WaitForSeconds(timeout);
        active = false;
        ActivateActives();
    }

    IEnumerator ContinuousCoroutine()
    {
        active = true;
        while (active)
        {
            if (!CheckRecieves())
                break;
            ActivateActives();
            yield return new WaitForSeconds(.1f);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        Activate();
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (buttontype == ButtonType.Hold)
            UnActivate();
    }
}
