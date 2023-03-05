using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class VoiceOver : MonoBehaviour
{
    public string folderName = "Replica";
    public string filePrefix = "Line_";

    //Audio source
    public AudioSource source;

    //List of all AudioClips
    public List<AudioClip> clips;

    public TextAsset textAsset;
    public List<string> lines;

    public float timeBeforeStart = 5f;
    public float timeBetweenLines = 1f;

    public TMP_Text tmp;

    void Start()
    {
        source = GetComponent<AudioSource>();
        lines = new List<string>(textAsset.text.Split('\n'));
        StartVoices();
    }

    void StartVoices()
    {
        StartCoroutine(PlayAudioSequentially());
    }

    IEnumerator PlayAudioSequentially()
    {
        yield return new WaitForSeconds(timeBeforeStart);

        //1.Loop through each AudioClip
        for (int i = 0; i < clips.Count; i++)
        {
            // Change text
            tmp.text = lines[i];

            //2.Assign current AudioClip to audiosource
            source.clip = clips[i];

            //3.Play Audio
            source.Play();

            //4.Wait for it to finish playing
            while (source.isPlaying)
            {
                yield return null;
            }

            yield return new WaitForSeconds(timeBetweenLines);
            //5. Go back to #2 and play the next audio in the adClips array
        }
    }
}
