using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SimpleAttach : MonoBehaviour
{
    public SteamVR_Action_Boolean PullLatch, Aim, Extinguishing;
    public Animator anim;
    public AudioClip[] audioClips;
    public ParticleSystem Particle;
    public ParticleSystem[] TotalFirePS;
    public Raycast raycast;
    public float ExtinguishingTime = 15;
    public GameObject Gauge;
    private bool debug;
    public bool ResetTube, Reset;
    public float SlerpTime = 15f;
    public bool Finished, Useful;
    private bool PlayingAudio, PlayingAudioII;
    private AudioSource audioSource;
    private Interactable interactable;
    public InputControl inputControl;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        interactable = GetComponent<Interactable>();
        ExtinguishingTime = ExtinguishingTime / (TotalFirePS.Length + 1);
    }

    private void OnHandHoverBegin(Hand hand)
    {
        hand.ShowGrabHint();
    }

    private void OnHandHoverEnd(Hand hand)
    {
        hand.HideGrabHint();
    }

    void Update()
    {
        if (interactable.attachedToHand != null)
        {
            SteamVR_Input_Sources source = interactable.attachedToHand.handType;
            if (!PlayingAudio)
            {
                audioSource.PlayOneShot(audioClips[0]);
                PlayingAudio = true;
            }

            if (!Extinguishing[source].stateDown && !anim.GetBool("Aim"))
            {
                Particle.Stop();
            }

            if (PullLatch[source].stateDown)                     //X;PullLatch
            {
                if (!PlayingAudioII && !anim.GetBool("PullLatch") && inputControl.ControlUI[3].activeInHierarchy)
                {
                    anim.SetBool("PullLatch", true);
                    audioSource.PlayOneShot(audioClips[2]);
                    PlayingAudioII = true;
                }
            }


            if (Aim[source].stateDown && !anim.GetBool("PullLatch"))
            {
                ResetTube = true;
            }
            if (Aim[source].stateUp && !anim.GetBool("PullLatch"))
            {
                ResetTube = false;
            }

            if (!ResetTube)
            {
                Reset = false;
                if (!anim.GetBool("Aim") && Aim[source].stateDown && anim.GetBool("PullLatch"))         //Y;Aim
                {
                    anim.SetBool("Aim", true);
                }
                if (anim.GetBool("Aim") && Aim[source].stateUp && anim.GetBool("PullLatch"))
                {
                    anim.SetBool("Aim", false);
                }
            }
            if (ResetTube)
            {
                Reset = true;
                if (!anim.GetBool("Aim") && Aim[source].stateUp && anim.GetBool("PullLatch"))
                {
                    anim.SetBool("Aim", true);
                }
                if (anim.GetBool("Aim") && Aim[source].stateDown && anim.GetBool("PullLatch"))
                {
                    anim.SetBool("Aim", false);
                }
            }

            if (Useful)
            {
                SlerpTime -= Time.deltaTime;
                if (SlerpTime < 0)
                {
                    SlerpTime = 0;
                    audioSource.panStereo = 0;
                    Particle.Stop();
                    audioSource.Stop();
                    Finished = true;
                }
                Gauge.transform.localRotation = Quaternion.Slerp(Gauge.transform.localRotation,
                    Quaternion.Euler(0, 48f, 0), SlerpTime * 0.00025f);

                if (raycast.RayCastedParticleSystem != null)                      //*ColorOverLifeTime.color.LerpAlphaKey
                {
                    var colorLifeTime = raycast.RayCastedParticleSystem.colorOverLifetime;
                    colorLifeTime.enabled = true;

                    if (colorLifeTime.color.gradient.alphaKeys[1].time > 0.002f)
                    {
                        float Alpha = colorLifeTime.color.gradient.alphaKeys[1].alpha - Time.deltaTime / ExtinguishingTime;
                        float time = colorLifeTime.color.gradient.alphaKeys[1].time - Time.deltaTime / ExtinguishingTime;

                        Gradient gradient = new Gradient();
                        gradient.SetKeys(
                            new GradientColorKey[]
                            {
                                new GradientColorKey(Color.white, 0.0f),new GradientColorKey(Color.white, 1.0f)
                            },
                            new GradientAlphaKey[]
                            {
                                new GradientAlphaKey(1.0f, 0.0f),new GradientAlphaKey(Alpha, time)
                            });

                        colorLifeTime.color = gradient;
                    }
                }
            }
            
            if (Extinguishing[source].stateDown && anim.GetBool("Aim"))     //holdA;Extinguishing
            {
                if (!Finished)
                {
                    anim.SetBool("Extinguishing", true);
                    Particle.Play();

                    Useful = true;

                    if (PlayingAudioII)
                    {
                        audioSource.panStereo = -0.2f;
                        if (audioSource.panStereo == -0.2f)
                        {
                            audioSource.PlayOneShot(audioClips[3]);
                            PlayingAudioII = false;
                        }
                    }
                }
                if (Finished)
                {
                    anim.SetBool("Extinguishing", true);
                    audioSource.panStereo = 0;
                    Particle.Stop();
                    audioSource.Stop();
                }
            }
            if (Extinguishing[source].stateUp && anim.GetBool("Aim"))
            {
                anim.SetBool("Extinguishing", false);
                Useful = false;
                PlayingAudioII = true;
                audioSource.panStereo = 0;
                Particle.Stop();
                audioSource.Stop();
            }
        }
        else
        {
            if (!Reset)
            {
                if (anim.GetBool("Aim"))
                {
                    anim.SetBool("Aim", false);
                    ResetTube = true;
                }
            }
            if (Reset)
            {
                if (anim.GetBool("Aim"))
                {
                    anim.SetBool("Aim", false);
                    ResetTube = false;
                }
            }
            Particle.Stop();
            if (PlayingAudio)
            {
                audioSource.PlayOneShot(audioClips[1]);
                PlayingAudio = false;
            }
        }
    }
}
