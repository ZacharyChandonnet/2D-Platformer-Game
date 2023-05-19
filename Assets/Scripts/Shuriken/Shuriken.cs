using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #Synthèse_ALBERT
/// Classe qui permet de gérer le shuriken
/// Auteur: Albert Jannard
/// Commentaire: Albert Jannard
/// </summary>
public class Shuriken : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioClip _shurikenSon; // Son du shuriken
    private const string ENNEMI_TAG = "Ennemi"; // Tag des ennemis
    private const string TUILE_TAG = "Tuile"; // Tag des tuiles
    [SerializeField] SOPerso _donneesPerso; // Données du personnage
    private float _dommageShuriken; // Dommage infligé par le shuriken

    [SerializeField] Ennemi[] _ennemi;

    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _dommageShuriken = _donneesPerso.dommageShuriken;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if(_dommageShuriken>=20)
        {
            // Change la couleur du shuriken en rouge
            _spriteRenderer.color = new Color(1f, 0f, 0f, 1f);
        }
        else
        {
            // si non change la couleur du shuriken en blanc
            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
    }
    /// <summary>
    /// Détruit le shuriken lorsqu'il entre en collision avec un ennemi ou une tuile.
    /// </summary>
    /// <param name="other">Le collider de l'objet entrant en collision avec le shuriken.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
            
        if (other.CompareTag(ENNEMI_TAG))
        {
            // Récupère le script de l'ennemi
            Ennemi ennemi = other.gameObject.GetComponent<Ennemi>();
            if (ennemi != null)
            {
                // Inflige des dégâts à l'ennemi
                ennemi.PrendreDegats(_dommageShuriken);

                // Détruit le shuriken
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag(TUILE_TAG))
        {
            // Détruit le shuriken
            Destroy(gameObject);
            
            GestAudio.instance.JouerEffetSonore(_shurikenSon); // #synthese_ALBERT
        }
    }
}
