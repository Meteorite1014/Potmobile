﻿using RoR2;
using System;
using System.Collections;
using UnityEngine;

//https://github.com/GnomeModder/EnforcerMod/blob/master/EnforcerMod_VS/Nemesis/SquashedComponent.cs
//Credits to the EnforcerGang for lending me this code!
namespace Potmobile.Components
{
    public class SquashedComponent : MonoBehaviour
    {
        public GameObject triggerer;
        public float speed = 5f;
        //public float squashMult = 1f;
        private Vector3 originalScale;
        public static float squashDuration = 20f;
        private HealthComponent health = null;

        private float graceTimer;
        public static float baseGraceTimer = 0.5f;

        private bool triggeredSquash = false;
        GameObject model;

        public BodyIndex GetBodyIndex()
        {
            if (health && health.body) return health.body.bodyIndex;
            return BodyIndex.None;
        }

        public void ResetGraceTimer()
        {
            if (!triggeredSquash)
            {
                graceTimer = baseGraceTimer;
            }
        }

        public void Awake()
        {
            graceTimer = baseGraceTimer;

            health = base.GetComponent<HealthComponent>();
            CharacterBody body = base.GetComponent<CharacterBody>();
            if (body.modelLocator && body.modelLocator.modelTransform) model = body.modelLocator.modelTransform.gameObject;
            if (!health || !body || !model) Destroy(this);
        }

        public void FixedUpdate()
        {
            if (!triggeredSquash)
            {
                if (health.alive && graceTimer > 0f)
                {
                    graceTimer -= Time.fixedDeltaTime;
                }
                else
                {
                    if (health.alive)
                    {
                        Destroy(this);
                    }
                    else
                    {
                        triggeredSquash = true;
                        StartSquash();
                    }
                }
            }
        }

        private void StartSquash()
        {
            originalScale = model.transform.localScale;
            model.transform.localScale = new Vector3(1.25f * originalScale.x, 0.05f * originalScale.y, 1.25f * originalScale.z);
            StartCoroutine("EndSquash");
        }

        IEnumerator EndSquash()
        {
            yield return new WaitForSeconds(squashDuration);

            float t = 0f;
            while (t < 1f)
            {
                t += speed * Time.deltaTime;
                model.transform.localScale = Vector3.Lerp(model.transform.localScale, originalScale, t);

                yield return 0;
            }

            transform.localScale = originalScale;
            Destroy(this);

            yield return null;
        }
    }
}
