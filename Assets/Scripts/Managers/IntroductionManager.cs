using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class IntroductionManager : MonoBehaviour
{
    [Tooltip("Time in seconds for the intro to last")]
    public float totalTimeOfIntroduction;

    [Tooltip("Time in seconds to fade the main characters out")]
    public float timeToStartFade;

    public float fadeSpeed = 1f;

    public float currentTime;

    public List<GameObject> akaAndGugak;
    private List<Material> materials = new List<Material>();
    public List<GameObject> attatchedObjects;

    public float pingPongSpeed = 2f;
    private bool fading;
    float emission = -10;
    public Color initialEmission;
    public Color secondEmission;

    private bool intro = true;
    public float timeBeforeDeleteObjects = 5f;

    public Transform startLocationGame;
    public float fadeToStart = 5f;
    public FadeCamera fadeCamera;
    public GameObject player;
    GameStateManager gameStateManager;

    private void Start()
    {
        gameStateManager = GetComponent<GameStateManager>();
        foreach (GameObject go in akaAndGugak)
        {
            SkinnedMeshRenderer r = go.GetComponent<SkinnedMeshRenderer>();
            //Debug.Log(r.materials[0]);
            materials.Add(r.materials[0]);
            materials.Add(r.materials[1]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (intro)
        {
            currentTime += Time.deltaTime;
            foreach(Material m in materials)
            {
                LerpEmmisionIntensity(m);
            }

            if (currentTime > totalTimeOfIntroduction - timeToStartFade && fading == false)
            {
                fading = true;
                foreach (Material m in materials)
                {
                    StartCoroutine(FadeOut(1, 0, timeToStartFade, m));
                }
                
            }
            else if(currentTime > totalTimeOfIntroduction && intro == true) // This gets called when the intro is finished
            {
                EndIntro();
            }
        }
    }

    void LerpEmmisionIntensity(Material mat)
    {
        emission = Mathf.PingPong(Time.time * pingPongSpeed, 1);
        mat.SetColor("_Emission", Color.Lerp(initialEmission, secondEmission, emission));
    }

    IEnumerator FadeOut(float startValue, float endValue, float lerpDuration, Material mat)
    {
        float fade = startValue;
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            fade = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            mat.SetFloat("_Opacity", fade);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        fade = endValue;
        mat.SetFloat("opacity", fade);
    }
    public void EndIntro()
    {
        Destroy(akaAndGugak[0].transform.parent.gameObject);
        Destroy(akaAndGugak[1].transform.parent.gameObject);
        GameStateManager.Instance.ChangeGameState(1);
        intro = false;
        foreach (GameObject go in attatchedObjects)
        {
            StartCoroutine(DetatchObjects(go, timeBeforeDeleteObjects));
        }
    }

    IEnumerator DetatchObjects(GameObject go, float timeBeforeDeleteObjects)
    {
        Rigidbody r = go.GetComponent<Rigidbody>();
        ParentConstraint pc = go.GetComponent<ParentConstraint>();
        r.isKinematic = false;
        pc.enabled = false;
        float timeElapsed = 0;
        while(timeElapsed < timeBeforeDeleteObjects)
        {
            timeElapsed += Time.deltaTime;
            yield return null;            
        }
        Destroy(go);
        StartCoroutine(MoveToStart());
    }

    IEnumerator MoveToStart()
    {
        fadeCamera.Reset();
        player.transform.position = startLocationGame.position;
        gameStateManager.ChangeGameState(2);
        yield return null;
    }
}
