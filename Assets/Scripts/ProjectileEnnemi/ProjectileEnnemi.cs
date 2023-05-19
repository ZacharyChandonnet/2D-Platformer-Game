using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #Synthèse_ALBERT
/// Classe qui permet de gérer le projectile ennemi
/// Auteur: Albert Jannard
/// Commentaire: Albert Jannard
/// </summary>
public class ProjectileEnnemi : MonoBehaviour
{
    private const string PLAYER_TAG = "Player"; // Tag des ennemis
    private const string TUILE_TAG = "Tuile"; // Tag des tuiles

    int _projectileDamage = 1; // Dommage infligé par le projectile
    [SerializeField] protected SOPerso _donneesPerso;

    /// <summary>
    /// Détruit le projectile lorsqu'il entre en collision avec un ennemi ou une tuile.
    /// </summary>
    /// <param name="other">Le collider de l'objet entrant en collision avec le projectile.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            if (_donneesPerso != null)
            {
                // Inflige des dégâts à l'perso
                _donneesPerso.vie -= _projectileDamage;

                // Détruit le projectile
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag(TUILE_TAG))
        {
            // Détruit le projectile
            Destroy(gameObject);
        }
    }
}
