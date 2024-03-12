using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
namespace Dev
{
    public class Initializer : MonoBehaviour
    {
        public Canvas MainCanvas;
        public EventSystem EventSystem;
        public GraphicRaycaster GraphicRaycaster;
        public AudioSource BGMSource;
        public AudioSource FXSource;
        public AudioSource UISoundSource;
        public TimerManager TimerManager;
        public GameObject TouchEffect;
    }
}