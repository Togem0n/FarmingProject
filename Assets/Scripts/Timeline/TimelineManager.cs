using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineManager : SingletonMonoBehaviour<TimelineManager>
{

    [SerializeField] private List<PlayableDirector> playableDirectors;
    
    [SerializeField] private List<TimelineAsset> timelines;

    public TimelineAsset timelineToPlay = null;

    public bool dialogueToPlay = false;

    private void Start()
    {
        EventHandler.AfterSceneLoadFadeInEvent += PlayTimeline;
    }

    private void OnEnable()
    {
        playableDirectors[0].played += OnPlayableDirectorPlayed;
        playableDirectors[0].stopped += OnPlayableDirectorStopped;
    }

    private void OnDisable()
    {
        playableDirectors[0].played -= OnPlayableDirectorPlayed;
        playableDirectors[0].stopped -= OnPlayableDirectorStopped;
    }

    private void Update()
    {
        
    }

    public void PlayTimeline()
    {
        if(timelineToPlay != null)
        {
            playableDirectors[0].Play(timelineToPlay);
        }
        else
        {
            Player.Instance.EnablePlayerInput();
        }
    }

    public void OnPlayableDirectorPlayed(PlayableDirector playableDirector)
    {
        Debug.Log("play timeline");
        Player.Instance.DisablePlayerInput();
    }

    public void OnPlayableDirectorStopped(PlayableDirector playableDirector)
    {
        Debug.Log("stop timeline");
        timelineToPlay = null;
        Player.Instance.EnablePlayerInput();
    }

    public void SetTimelineToPlay(string timelineToPlay, bool dialogueToPlay)
    {
        foreach(TimelineAsset timeline in timelines)
        {
            if(timeline.name == timelineToPlay)
            {
                this.timelineToPlay = timeline;
                this.dialogueToPlay = dialogueToPlay;
            }
        }
    }

    public void PauseTimeline()
    {
        playableDirectors[0].Pause();
    }

    public void ResumeTimeline()
    {
        playableDirectors[0].Resume();
    }
}
