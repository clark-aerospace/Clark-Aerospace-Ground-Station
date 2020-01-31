using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Test2020 : MonoBehaviour
{
    public ParticleSystem newDataReceivedParticleSys;

    public void Start() {
        // ArduinoReciever.reciever.unityEvent.AddListener(ReceivedInfo);
    }

    public void ReceivedInfo() {
        newDataReceivedParticleSys.Emit(1);
    }
}