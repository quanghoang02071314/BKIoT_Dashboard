using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class BKIoTManager : MonoBehaviour
{
    public Slider Layer;

    [SerializeField]
    private CanvasGroup _canvasLayer1;
    [SerializeField]
    private CanvasGroup _canvasLayer2;
    [SerializeField]
    private CanvasGroup _errorLayer;

    private Tween tweenFade;

    public void Fade(CanvasGroup _canvas, float endValue, float duration, TweenCallback onFinish)
    {
        if (tweenFade != null)
            tweenFade.Kill(false);

        tweenFade = _canvas.DOFade(endValue, duration);
        tweenFade.onComplete += onFinish;
    }

    public void FadeIn(CanvasGroup _canvas, float duration)
    {
        Fade(_canvas, 1f, duration, () =>
        {
            _canvas.interactable = true;
            _canvas.blocksRaycasts = true;
        });
    }

    public void FadeOut(CanvasGroup _canvas, float duration)
    {
        Fade(_canvas, 0f, duration, () =>
        {
            _canvas.interactable = false;
            _canvas.blocksRaycasts = false;
        });
    }

    IEnumerator _IESwitchLayer()
    {
        if (_canvasLayer1.interactable == true)
        {
            FadeOut(_canvasLayer1, 0.25f);
            yield return new WaitForSeconds(0.5f);
            FadeIn(_canvasLayer2, 0.25f);
        }
        else{
            FadeOut(_canvasLayer2, 0.25f);
            yield return new WaitForSeconds(0.5f);
            FadeIn(_canvasLayer1, 0.25f);
        }
    }

    IEnumerator _Error()
    {
        if (_canvasLayer1.interactable == true)
        {
            FadeOut(_canvasLayer1, 0.25f);
            yield return new WaitForSeconds(0.5f);
            FadeIn(_errorLayer, 0.25f);
        }
        else{
            FadeOut(_errorLayer, 0.25f);
            yield return new WaitForSeconds(0.5f);
            FadeIn(_canvasLayer1, 0.25f);
            Layer.value = 0;
        }
    }

    public void LayerError()
    {
        StartCoroutine(_Error());
    }

    public void SwitchLayer()
    {
        if (Layer.value == 1)
        {
            StartCoroutine(_IESwitchLayer());
        }
        else if (Layer.value == 0.5f)
        {
            StartCoroutine(_Error());
        }
    }
}

