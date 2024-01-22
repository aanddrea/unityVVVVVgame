using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txtMesh;
    private int trinketsCollected = 0;


    public void AddTrinket()
    {
        trinketsCollected++;
        if (trinketsCollected < 3)
        {
            txtMesh.text = "collect trinkets: " + trinketsCollected.ToString();
        }
        else
        {
            txtMesh.text = "All trinkets collected! YOU WIN";
        }
    }
}
