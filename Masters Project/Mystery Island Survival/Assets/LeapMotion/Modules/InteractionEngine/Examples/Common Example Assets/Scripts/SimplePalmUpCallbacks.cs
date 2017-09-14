/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2017.                                 *
 * Leap Motion proprietary and  confidential.                                 *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Leap.Unity.Examples {

  [AddComponentMenu("")]
  public class SimplePalmUpCallbacks : MonoBehaviour {

    public Transform toPalmUp;

    private bool _initialized = false;
    private bool _isPalmUp = false;

    public UnityEvent OnBeginPalmUp;
    public UnityEvent OnEndPalmUp;

    void Start() {
      if (toPalmUp != null) initialize();
    }

    private void initialize() {
      // Set "_isFacingCamera" to be whatever the current state ISN'T, so that we are
      // guaranteed to fire a UnityEvent on the first initialized Update().
      _isPalmUp = !GetPalmUp(toPalmUp, Camera.main);
      _initialized = true;
    }

    void Update() {
      if (toPalmUp != null && !_initialized) {
        initialize();
      }
      if (!_initialized) return;

      if (GetPalmUp(toPalmUp, Camera.main, _isPalmUp ? 0.77F : 0.82F) != _isPalmUp) {
        _isPalmUp = !_isPalmUp;

        if (_isPalmUp) {
          OnBeginPalmUp.Invoke();
        }
        else {
          OnEndPalmUp.Invoke();
        }
      }
    }

    public static bool GetPalmUp(Transform facingTransform, Camera camera, float minAllowedDotProduct = 0.8F) {
      return Vector3.Dot((camera.transform.position - facingTransform.position).normalized, facingTransform.forward) > minAllowedDotProduct;
    }

  }

}
