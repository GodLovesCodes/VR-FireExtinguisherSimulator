using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Raycast : MonoBehaviour
{
    public float RayLength;
    public GameObject FireExtinguisher;
    public LayerMask layerMaskInteract;
    public SimpleAttach simpleAttach;
    public Text ScoreText;
    public float Score = 0;
    private bool checkScore, checkScore2, checkScore3;
    public ParticleSystem RayCastedParticleSystem;

    public FireLight fireLight;
    public float intensityValue;
    public Color WhiteColor, FadeColor;
    public bool Lerp;

    void Start()
    {
        intensityValue = fireLight.GetComponent<Light>().intensity;
    }

    void Update()
    {
        ScoreText.text = Score.ToString("0.00");

        if (Lerp)
        {
            ScoreText.color = Color.Lerp(ScoreText.color, WhiteColor, 5 * Time.deltaTime);
        }
        else
        {
            ScoreText.color = Color.Lerp(ScoreText.color, FadeColor, 5 * Time.deltaTime);
        }

        if (simpleAttach.anim.GetBool("PullLatch"))
        {
            if (!checkScore && !Lerp)
            {
                Score = Score + 20;
                checkScore = true;
                Lerp = true;
            }
            if (ScoreText.color == WhiteColor)
            {
                Lerp = false;
            }
        }

        if (simpleAttach.anim.GetBool("Aim"))
        {
            if (!checkScore2 && !Lerp)
            {
                Score = Score + 20;
                checkScore2 = true;
                Lerp = true;
            }
            if (ScoreText.color == WhiteColor)
            {
                Lerp = false;
            }
        }

        RaycastHit hit;
        Vector3 fwd = FireExtinguisher.transform.TransformDirection(Vector3.left);

        if (Physics.Raycast(FireExtinguisher.transform.position, fwd, out hit, RayLength, layerMaskInteract.value))
        {
            RayCastedParticleSystem = hit.collider.GetComponentInParent<ParticleSystem>();

            if (simpleAttach.anim.GetBool("Extinguishing") && simpleAttach.Useful)
            {
                if (!checkScore3)
                {
                    if (simpleAttach.TotalFirePS.Length == 1)
                    {
                        Score += 8 * Time.deltaTime;
                        Lerp = true;
                    }

                    intensityValue -= Time.deltaTime * simpleAttach.ExtinguishingTime * 0.18f;
                    fireLight.minValue = intensityValue - 0.5f;
                    fireLight.maxValue = intensityValue + 0.5f;
                    if (intensityValue <= 0)
                    {
                        intensityValue = 0;
                        RayCastedParticleSystem.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                Lerp = false;
            }
        }
        else
        {
            RayCastedParticleSystem = null;
        }
    }
}
