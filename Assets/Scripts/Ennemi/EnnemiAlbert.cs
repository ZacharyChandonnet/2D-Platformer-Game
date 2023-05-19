using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// #Synthèse_ALBERT
/// Classe qui permet de gérer l'ennemi Albert
/// Auteur: Albert Jannard
/// Commentaire: Albert Jannard
/// </summary>
public class EnnemieAlbert : Ennemi
{

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 5f;

    [Header("Temps entre les attaques")]
    public float minTimeBetweenAttacks = 2f;
    public float maxTimeBetweenAttacks = 5f;

    private GameObject player;
    private float timeUntilNextAttack;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        timeUntilNextAttack = Random.Range(minTimeBetweenAttacks, maxTimeBetweenAttacks);
    }

    void Update()
    {
        if (timeUntilNextAttack <= 0)
        {
            // Tirer un projectile
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Calcule la direction vers le joueur
            Vector2 direction = (player.transform.position - transform.position).normalized;

            // Inverse le scale du projectile s'il va vers la gauche
            if (direction.x < 0)
            {
                Vector3 scale = projectile.transform.localScale;
                scale.x = -scale.x;
                projectile.transform.localScale = scale;
            }

            // Applique une vitesse au projectile dans la direction du joueur
            projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

            // Désactiver la gravité sur le projectile
            projectile.GetComponent<Rigidbody2D>().gravityScale = 0f;

            // Détruire le projectile après 3 secondes
            Destroy(projectile, 2f);

            timeUntilNextAttack = Random.Range(minTimeBetweenAttacks, maxTimeBetweenAttacks);
        }
        else
        {
            timeUntilNextAttack -= Time.deltaTime;
        }
    }

}

