using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [Header("Caméra")]
    [SerializeField] private Transform cameraPrincipal;
    [SerializeField] private Vector3 camPositionDepart;
    [SerializeField] private Vector2 distance;

    [Header("Fond Parallax")]
    [SerializeField] private GameObject[] Fond_Parallax;
    [SerializeField] private Material[] mat; 
    [SerializeField] private float[] vitesseFond;

    [Header("Paramètres")]
    [Range(0f, 0.05f)]
    public float vitesseParallax;

    float fondLePlusLoin;

    void Start()
    {
        cameraPrincipal = Camera.main.transform;
        camPositionDepart = cameraPrincipal.position;

        int nbDeFonds = transform.childCount;
        mat = new Material[nbDeFonds];
        vitesseFond = new float[nbDeFonds];
        Fond_Parallax = new GameObject[nbDeFonds];

        for (int i = 0; i < nbDeFonds; i++)
        {
            Fond_Parallax[i] = transform.GetChild(i).gameObject;
            mat[i] = Fond_Parallax[i].GetComponent<Renderer>().material;

        }
        CalculerVitesseFonds(nbDeFonds);
    }

    void CalculerVitesseFonds(int nbDeFonds)
    {
        for (int i = 0; i < nbDeFonds; i++) // Trouver l'arrière-plan le plus éloigné
        {
            if ((Fond_Parallax[i].transform.position.z - cameraPrincipal.position.z) > fondLePlusLoin)
            {
                fondLePlusLoin = Fond_Parallax[i].transform.position.z - cameraPrincipal.position.z;
            }

        }

        for (int i = 0; i < nbDeFonds; i++) // Définir la vitesse des arrière-plans
        {
            vitesseFond[i] = 1 - (Fond_Parallax[i].transform.position.z - cameraPrincipal.position.z) / fondLePlusLoin;
        }
    }



    private void LateUpdate()
    {
        distance = cameraPrincipal.position - camPositionDepart;
        transform.position = new Vector3(cameraPrincipal.position.x, transform.position.y, 0);


        for (int i = 0; i < Fond_Parallax.Length; i++)
        {
            float speedX = vitesseFond[i] * vitesseParallax;
            mat[i].SetTextureOffset("_MainTex", new Vector2(distance.x * speedX, 0));
        }
    }


}
