using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem
{
    const string defaultSceneName = "MainMenu";
    const float delay = 1.5f;

    static bool m_starting = false;

    public static void changeScene(string sceneName, bool instant = false, Action finishedCallback = null)
    {
        if (m_starting)
            return;
        m_starting = true;

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("Can't load scene " + sceneName + ". Back to main menu");
            sceneName = defaultSceneName;
        }

        Time.timeScale = 1.0f;

        Event<ShowLoadingScreenEvent>.Broadcast(new ShowLoadingScreenEvent(true));

        if (!instant)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                var operation = SceneManager.LoadSceneAsync(sceneName);
                execChangeScene(operation, finishedCallback);
            });
        }
        else 
        {
            var operation = SceneManager.LoadSceneAsync(sceneName);
            execChangeScene(operation, finishedCallback);
        }
    }

    static void execChangeScene(AsyncOperation operation, Action finishedCallback)
    {
        if (!operation.isDone)
            DOVirtual.DelayedCall(0.1f, () => execChangeScene(operation, finishedCallback));
        else
        {
            Event<ShowLoadingScreenEvent>.Broadcast(new ShowLoadingScreenEvent(false));
            m_starting = false;
            if (finishedCallback != null)
                finishedCallback();
        }
    }

    public static void ChangeWorld(WorldObject world, string spawnName, Action finishedCallback = null)
    {
        if (m_starting)
            return;

        m_starting = true;

        Time.timeScale = 1.0f;
        Event<ShowLoadingScreenEvent>.Broadcast(new ShowLoadingScreenEvent(true));

        DOVirtual.DelayedCall(delay, () =>
        {
            Event<SetWorldEvent>.Broadcast(new SetWorldEvent(null));
            Vector2 spawn;
            if (world.GetSpawnPos(spawnName, out spawn))
                Event<TeleportPlayerEvent>.Broadcast(new TeleportPlayerEvent(spawn));
            Event<SetWorldEvent>.Broadcast(new SetWorldEvent(world));

            Event<ShowLoadingScreenEvent>.Broadcast(new ShowLoadingScreenEvent(false));
        });

    }
}