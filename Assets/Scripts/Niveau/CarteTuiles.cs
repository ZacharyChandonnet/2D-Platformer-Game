using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Classe gère le traitement des tuiles de chaque Salle afin d'analyser ou non si on doit garder une tuile selon des probabilitées aléatoires 
/// Lorsqu'on parle de garder une tuile ou non, on parle des autres TilesMap dans CartesTuiles afin de savoir si on la garde ou pas grâce aux probabilités
/// Auteurs du code: Zachary Chandonnet 
/// Auteur des commentaires: Zachary Chandonnet
/// </summary>

public class CarteTuile : MonoBehaviour
{
    // Définit la probabilité que chaque tuile soit conservée(TileMap). Si cette probabilité est inférieure au résultat aléatoire d'un dé (entre 0 et 100).
    // On a donc une chance sur 2 de garder une tuile (CarteTuiles)
   [SerializeField] int probabiliteDeConservation = 50; // La probabilité de conservation des tuiles (de base mais change selon les différentes cartes)

   private void Awake() 
   {

    Tilemap tm = GetComponent<Tilemap>();
    BoundsInt bounds = tm.cellBounds;
    Niveau niveau = GetComponentInParent<Niveau>();
    int resultatDe = Random.Range(0,100);
    Vector3Int decalage = Vector3Int.FloorToInt(transform.position);

    // On vérifie ici si notre résultat de dé est inférieur à notre prob de conservation afin de déterminer si on garde la tuile en question ou non
    // C'est grâce au deux boucles for qu'on réussi à parcourir l'intégrité de nos salles avec les bounds.min & max (x,y)
    // On vient alors copier notre tuile selon notre position actuelle de la tuile, sinon on l'enlève(CarteTuiles)
    if (resultatDe < probabiliteDeConservation)
    {
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int pos = new Vector3Int(x,y,0);
                TraiterUneTuile(tm,niveau,pos, decalage); 
            }
        }
    }else{gameObject.SetActive(false);}
}

    // Cette méthode permet de traiter tuile par tuile notre tileMap généré. On prend en compte notre TileMap, notre niveau, notre position
    // ainsi qu'un décalage pour éviter l'empilation. On récupère notre tuile à la position donnée si elle n'est pas null on l'ajoute à notre TileMap du niveau
    // avec un décalage. Sinon on l'enlève de notre tm.
    // Lorsqu'on parle d'un décalage, c'est d'une salle à une autre afin d'éviter d'avoir tout dans la même salle
    public void TraiterUneTuile(Tilemap tm, Niveau niveau, Vector3Int pos, Vector3Int decalage)
    {
        TileBase tuile = tm.GetTile(pos);

        if (tuile != null) niveau.tileMap.SetTile(pos + decalage , tuile);
        else{tm.SetTile(pos, null);}
            
    }    
}
