using UnityEngine;

public enum Skill_Upgrade_Type
{
    None,
    // --- - Dash Upgrades ----
    Dash,
    Dash_CloneOnStart,              // Create a clone when dash starts
    Dash_CloneOnStartAndArrival,    // Create a clone when dash starts and ends
    Dash_ShardOnStart,              // Create a shard when dash starts
    Dash_ShardOnStartAndArrival,     // Create a shard when dash starts and ends


    // --- - Time Shard Upgrades ----
    Shard,                  //The Shard explodes when touched by an enemy or time runs out.
    Shard_MoveToEnemy,      // The Shard moves toward the nearest enemy.
    Shard_MultiCast,       // Shard ability can have up to N Charges. you can cast them all in a row.
    Shard_Teleport,         // You can teleport to the Shard's location.
    Shard_TeleportHPRewind, // When teleporting to the Shard, you rewind your HP to the value when the Shard was created.

    // ----- Sword Slash Upgrades ----
    SwordSlash,         // Basic Sword Slash ability, a big long range slash in front of the player.
    SwordSlash_Heal,    // Heal the player for a percentage of the damage dealt.
    SwordSlash_Cooldown, // Reduce the cooldown of all player skills by a percentage for each enemy hit.
    SwordSlash_Cleave,   // Each enemy hit creates a smaller cleave slash that hits other enemies around them.

    // --- - Time Echo Upgrades ----
    TimeEcho,               // Basic Time Echo ability, Create a clone of a player. It can take damage from Enemies.
    TimeEcho_SingleAttack,      // The Echo performs a single attack mimicking the player's last attack.
    TimeEcho_MultiAttack,       // The Echo performs a series of attacks mimicking the player's last combo.
    TimeEcho_ChanceToMultiply, // Each Echo has a chance to create an additional Echo upon attack.

    TimeEcho_HealWisp,      // When the Echo expires or is destroyed, it heals the player for a percentage of their max health.
    TimeEcho_CleanseWisp,    // When the Echo expires or is destroyed, it cleanses all negative status effects from the player.
    TimeEcho_CooldownWisp,   // When the Echo expires or is destroyed, it reduces the cooldown of all player skills by a percentage.

    // --- - Domain Expansion Upgrades ----
    Domain_SlowingDown,      // Enemies inside the domain have their movement speed reduced.
    Domain_EchoSpam,         // While in the domain, the player cant move but will spam Time Echo abilities.
    Domain_ShardSpam,        // While in the domain, the player cant move but will spam Time shard abilities.





    // --- - Sword Throw Upgrades ----

    //SwordThrow,         // Basic Sword Throw ability
    //SwordThrow_Spin,    // The sword spins while flying, dealing damage to all enemies in its path.
    //SwordThrow_Pierce,  // The sword pierces through enemies, hitting multiple targets in a straight line.
    //SwordThrow_Bounce,   // The sword bounces between enemies, hitting several targets before returning.
}
