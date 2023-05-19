using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Auteurs du code: Zachary Chandonnet 
/// Auteur des commentaires: Zachary Chandonnet
/// </summary>

public class Salle : MonoBehaviour
{
    [Header("Repères")]
    [SerializeField] private Transform _repere;
    [SerializeField] private Transform _reperePerso;


    // #synthese_ZACHARY
    // #synthese_Albert
    // On a changé la taille et les prefabs des salles afin de les optimiser pour notre jeu
    static Vector2Int _taille = new Vector2Int(32, 18); // La taille de nos salles seront de 32x18
    static public Vector2Int taille => _taille;
    public Color couleurFond = Color.red; // La couleur d'arrière-plan de la salle, par défaut est bleue


    /// <summary>
    /// #TP3 ZACHARY
    /// Fonction qui permet d'instantier notre modele sur notre repere, que ce soit une porte ou une clef
    /// </summary>
    /// <param name="modele"></param>
    /// <returns></returns>
    public Vector2Int PlacerSurRepere(GameObject modele)
    {
        Vector3 pos = _repere.position;
        Instantiate(modele, pos, Quaternion.identity, transform.parent);
        return Vector2Int.FloorToInt(pos);
    }

    /// <summary>
    /// #TP3 ZACHARY
    /// Fonction qui permet de transformer notre position du personnage sur notre repere personnage
    /// </summary>
    /// <param name="modele"></param>
    /// <returns></returns>
    public Vector2Int TransformSurRepere(GameObject modele)
    {
        Vector3 pos = _reperePerso.position;
        modele.transform.position = pos;
        return Vector2Int.FloorToInt(pos);
    }


    // On vient ici dessiner notre gizmos selon les dimensions de nos salles
    private void OnDrawGizmos()
    {
        Gizmos.color = couleurFond;
        Gizmos.DrawCube(transform.position, new Vector2(_taille.x, _taille.y));
    }
}
