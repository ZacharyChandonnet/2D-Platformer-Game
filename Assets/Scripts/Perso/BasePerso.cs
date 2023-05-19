using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe qui permet de détecter le sol du personnage.
/// Elle intéragit avec le script Perso afin de savoir quand celui-ci à le droit de sauter
/// Auteurs du code: Zachary Chandonnet 
/// Auteur des commentaires: Zachary Chandonnet
/// </summary>

public class BasePerso : MonoBehaviour
{
    [Header("Paramètres de la boîte")]
    [SerializeField] Vector2 _tailleBoite = new Vector2(0.5f, 0.1f);
    [SerializeField] LayerMask _layerMask;

    // TOUTES MES VARIABLES 
    protected bool _estAuSol = false;


    virtual protected void FixedUpdate()
    {
        DetecterSol();
    }

    /// <summary>
    /// Fonction qui permet de détecter si le personnage à atteint le sol ou non
    /// Pour ce faire, on utilise les coordonées de la boite qui a été créé, et on l'ajoute dans la variable hit grâce au OverlapBox
    /// Le layerMask est associé au sol
    /// Si la variable hit(collider) n'entre en contact avec aucun autre collider, nous ne sommes pas au sol 
    /// Il sera donc impossible de sauter à nouveau (dans le script Perso.cs)
    /// </summary>

    void DetecterSol()
    {
        Vector2 posCentreBoite = (Vector2)transform.position + new Vector2(0, -1.3f);
        Collider2D hit = Physics2D.OverlapBox(posCentreBoite, _tailleBoite, 0, _layerMask);
        _estAuSol = (hit != null);
    }

    /// <summary>
    /// Fonction qui permet de créer mon Gizmos
    /// On créé une boite qui à les même propriétés que mon collider juste en haut
    /// </summary>

    private void OnDrawGizmos()
    {
        Vector2 posCentreBoite = (Vector2)transform.position + new Vector2(0, -1.3f);
        Gizmos.DrawWireCube(posCentreBoite, _tailleBoite);
    }
}
