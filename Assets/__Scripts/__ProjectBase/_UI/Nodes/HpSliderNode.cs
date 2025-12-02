using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HpSliderNode : BaseNode
{
    public Slider hpSlider;
    public Slider reduceSlider;
    public Slider addSlider;

    [Header("Animation Settings")]
    [SerializeField]
    protected bool useAnimation = true;
    [SerializeField]
    protected float animationDuration = 0.5f;
    [SerializeField]
    protected LeanTweenType animationType = LeanTweenType.easeInOutQuad;
    
    protected float normalizedValue = 0f;

#if UNITY_EDITOR
    [ShowInInspector]
    [ProgressBar(0, 1f)]
    protected float Value
    {
        get => normalizedValue;
        set => InitValue(value);
    }

    [Button("Set Random Value")]
    protected void SetRandomValue()
    {
        float randomValue = Random.value;
        if (Application.isPlaying)
        {
            SetValue(randomValue);
        }
        else
        {
            InitValue(randomValue);
        }
    }
#endif

    public void InitValue(float normalizedValue)
    {
        normalizedValue = Mathf.Clamp01(normalizedValue);
        this.normalizedValue = normalizedValue;
        SetToFinalValue();
    }
    
    public void SetValue(float normalizedValue)
    {
        normalizedValue = Mathf.Clamp01(normalizedValue);
        if (this.normalizedValue > normalizedValue)
        {
            SetValueReduce(normalizedValue);
        }
        else
        {
            SetValueAdd(normalizedValue);
        }
    }

    public void AddValue(float normalizedValue)
    {
        float newValue = this.normalizedValue + normalizedValue;
        newValue = Mathf.Clamp01(newValue);
        SetValueAdd(newValue);
    }

    public void ReduceValue(float normalizedValue)
    {
        float newValue = this.normalizedValue - normalizedValue;
        newValue = Mathf.Clamp01(newValue);
        SetValueReduce(newValue);
    }

    protected void SetValueAdd(float normalizedValue)
    {
        if (!useAnimation)
        {
            SetValueAddNoAnimation(normalizedValue);
        }
        else
        {
            SetValueAddAnimation(normalizedValue);
        }
        
        this.normalizedValue = normalizedValue;
    }

    protected virtual void SetValueAddNoAnimation(float normalizedValue)
    {
        hpSlider.value = normalizedValue;
    }

    protected virtual void SetValueAddAnimation(float normalizedValue)
    {
        SetToFinalValue();
        addSlider.value = normalizedValue;
        LeanTween.value(
                hpSlider.gameObject, 
                this.normalizedValue, 
                normalizedValue, 
                animationDuration).
            setOnUpdate((float val) =>
            {
                hpSlider.value = val;
            }).setEase(animationType);
    }

    protected void SetValueReduce(float normalizedValue)
    {
        if (!useAnimation)
        {
            SetValueReduceNoAnimation(normalizedValue);
        }
        else
        {
            SetValueReduceAnimation(normalizedValue);
        }
        
        this.normalizedValue = normalizedValue;
    }

    protected virtual void SetValueReduceNoAnimation(float normalizedValue)
    {
        hpSlider.value = normalizedValue;
    }

    protected virtual void SetValueReduceAnimation(float normalizedValue)
    {
        SetToFinalValue();
        hpSlider.value = normalizedValue;
        LeanTween.value(
                reduceSlider.gameObject, 
                this.normalizedValue, 
                normalizedValue, 
                animationDuration).
            setOnUpdate((float val) =>
            {
                reduceSlider.value = val;
            }).setEase(animationType);
    }

    protected virtual void SetToFinalValue()
    {
        LeanTween.cancel(hpSlider.gameObject);
        hpSlider.value = normalizedValue;
        LeanTween.cancel(reduceSlider.gameObject);
        reduceSlider.value = 0;
        LeanTween.cancel(addSlider.gameObject);
        addSlider.value = 0;
        
    }
}
