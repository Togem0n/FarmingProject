using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public Weather currentWeather;

    protected override void Awake()
    {
        base.Awake();

        // set screen resolution

        currentWeather = Weather.dry;
    }
}
