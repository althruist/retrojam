using UnityEngine;
using DG.Tweening;
public class FOVSystem : MonoBehaviour
{
    private Player player1;
    private Player player2;

    public bool distanceMode = true;

    public static FOVSystem Instance;

    private void Awake()
    {
        Instance = this;

        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);

        foreach (Player p in players)
        {
            if(p.name == "Player1")
            {
                player1 = p;
            } else if(p.name == "Player2")
            {
                player2 = p;
            }
        }
    }

    public void setFOV(float fovSize, float duration, bool forceSet = false)
    {
        if(!forceSet && distanceMode){Debug.LogWarning("Attempted to set FOV when distance mode is on!"); return;};
        Instance.GetComponent<Camera>().DOOrthoSize(fovSize, duration).SetEase(Ease.Linear);
    }

    void Update()
    {
        if (distanceMode == false){return;};
        float distance = Vector3.Distance(player1.gameObject.transform.position, player2.gameObject.transform.position);
        setFOV(Mathf.Clamp(distance * 2, 5, 10), 2, true);
    }
}
