using UnityEngine;
using System;

public class LoopConductor : MonoBehaviour
{
  public static LoopConductor Instance;

  [Header("Length of one loop (in seconds)")]
  public double loopLengthSeconds = 4.0;

  private double loopStartDspTime;

  void Awake()
  {
    Instance = this;
  }

  void Start()
  {
    loopStartDspTime = AudioSettings.dspTime;
  }

  public double GetNextLoopStart()
  {
    double dspNow = AudioSettings.dspTime;
    double elapsed = dspNow - loopStartDspTime;

    double cycles = Math.Floor(elapsed / loopLengthSeconds);
    double nextStart = loopStartDspTime + (cycles + 1) * loopLengthSeconds;

    return nextStart;
  }
}
