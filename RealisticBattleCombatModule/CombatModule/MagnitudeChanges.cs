﻿using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using System;
using TaleWorlds.CampaignSystem;
using System.Reflection;
using TaleWorlds.Localization;
using TaleWorlds.Core.ViewModelCollection.Information;

namespace RBMCombat
{
    public class MagnitudeChanges
    {
        [HarmonyPatch(typeof(MissionCombatMechanicsHelper))]
        [HarmonyPatch("CalculateBaseMeleeBlowMagnitude")]
        public class CalculateBaseMeleeBlowMagnitudePatch
        {
            public static bool Prefix(ref float __result, in AttackInformation attackInformation, in MissionWeapon weapon, StrikeType strikeType, float progressEffect, float impactPointAsPercent, float exraLinearSpeed, bool doesAttackerHaveMount)
            {
                WeaponComponentData currentUsageItem = weapon.CurrentUsageItem;
                BasicCharacterObject attackerAgentCharacter = attackInformation.AttackerAgentCharacter;
                BasicCharacterObject attackerCaptainCharacter = attackInformation.AttackerCaptainCharacter;
                float num = MathF.Sqrt(progressEffect);

                if (strikeType == StrikeType.Thrust)
                {
                    exraLinearSpeed *= 1f;
                    float thrustWeaponSpeed = (float)weapon.GetModifiedThrustSpeedForCurrentUsage() / 11.7647057f * num;

                    if (weapon.Item != null && weapon.CurrentUsageItem != null)
                    {

                        Agent attacker = null;
                        foreach (Agent agent in Mission.Current.Agents)
                        {
                            if (attackInformation.AttackerAgentOrigin == agent.Origin)
                            {
                                attacker = agent;
                            }
                        }

                        if (attacker != null)
                        {
                            SkillObject skill = weapon.CurrentUsageItem.RelevantSkill;
                            int ef = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(attackerAgentCharacter, attackInformation.AttackerAgentOrigin, attacker.Formation, skill);
                            float effectiveSkillDR = Utilities.GetEffectiveSkillWithDR(ef);
                            switch (weapon.CurrentUsageItem.WeaponClass)
                            {
                                case WeaponClass.LowGripPolearm:
                                case WeaponClass.Mace:
                                case WeaponClass.OneHandedAxe:
                                case WeaponClass.OneHandedPolearm:
                                case WeaponClass.TwoHandedMace:
                                    {
                                        float swingskillModifier = 1f + (effectiveSkillDR / 1000f);
                                        float thrustskillModifier = 1f + (effectiveSkillDR / 1000f);
                                        float handlingskillModifier = 1f + (effectiveSkillDR / 700f);

                                        thrustWeaponSpeed = Utilities.CalculateThrustSpeed(weapon.Item.Weight, weapon.CurrentUsageItem.Inertia, weapon.CurrentUsageItem.CenterOfMass);
                                        thrustWeaponSpeed = thrustWeaponSpeed * 0.75f * thrustskillModifier * num;
                                        break;
                                    }
                                case WeaponClass.TwoHandedPolearm:
                                    {
                                        float swingskillModifier = 1f + (effectiveSkillDR / 1000f);
                                        float thrustskillModifier = 1f + (effectiveSkillDR / 1000f);
                                        float handlingskillModifier = 1f + (effectiveSkillDR / 700f);

                                        thrustWeaponSpeed = Utilities.CalculateThrustSpeed(weapon.Item.Weight, weapon.CurrentUsageItem.Inertia, weapon.CurrentUsageItem.CenterOfMass);
                                        thrustWeaponSpeed = thrustWeaponSpeed * 0.7f * thrustskillModifier * num;
                                        break;
                                    }
                                case WeaponClass.TwoHandedAxe:
                                    {
                                        float swingskillModifier = 1f + (effectiveSkillDR / 800f);
                                        float thrustskillModifier = 1f + (effectiveSkillDR / 1000f);
                                        float handlingskillModifier = 1f + (effectiveSkillDR / 700f);

                                        thrustWeaponSpeed = Utilities.CalculateThrustSpeed(weapon.Item.Weight, weapon.CurrentUsageItem.Inertia, weapon.CurrentUsageItem.CenterOfMass);
                                        thrustWeaponSpeed = thrustWeaponSpeed * 0.9f * thrustskillModifier * num;
                                        break;
                                    }
                                case WeaponClass.OneHandedSword:
                                case WeaponClass.Dagger:
                                case WeaponClass.TwoHandedSword:
                                    {
                                        float swingskillModifier = 1f + (effectiveSkillDR / 800f);
                                        float thrustskillModifier = 1f + (effectiveSkillDR / 800f);
                                        float handlingskillModifier = 1f + (effectiveSkillDR / 800f);

                                        thrustWeaponSpeed = Utilities.CalculateThrustSpeed(weapon.Item.Weight, weapon.CurrentUsageItem.Inertia, weapon.CurrentUsageItem.CenterOfMass);
                                        thrustWeaponSpeed = thrustWeaponSpeed * 0.7f * thrustskillModifier * num;
                                        break;
                                    }
                            }

                            float thrustMagnitude = 0f;
                            switch (weapon.CurrentUsageItem.WeaponClass)
                            {
                                case WeaponClass.OneHandedPolearm:
                                case WeaponClass.OneHandedSword:
                                case WeaponClass.Dagger:
                                case WeaponClass.Mace:
                                case WeaponClass.LowGripPolearm:
                                    {
                                        thrustMagnitude = Utilities.CalculateThrustMagnitudeForOneHandedWeapon(weapon.Item.Weight, effectiveSkillDR, thrustWeaponSpeed, exraLinearSpeed, attacker.AttackDirection);
                                        break;
                                    }
                                case WeaponClass.TwoHandedPolearm:
                                case WeaponClass.TwoHandedSword:
                                    {
                                        thrustMagnitude = Utilities.CalculateThrustMagnitudeForTwoHandedWeapon(weapon.Item.Weight, effectiveSkillDR, thrustWeaponSpeed, exraLinearSpeed, attacker.AttackDirection);
                                        break;
                                    }
                                default:
                                    {
                                        thrustMagnitude = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateStrikeMagnitudeForThrust(attackerAgentCharacter, attackerCaptainCharacter, thrustWeaponSpeed, weapon.Item.Weight, currentUsageItem, exraLinearSpeed, doesAttackerHaveMount);
                                        break;
                                    }
                            }

                            __result = thrustMagnitude;
                            return false;
                        }
                    }

                    __result = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateStrikeMagnitudeForThrust(attackerAgentCharacter, attackerCaptainCharacter, thrustWeaponSpeed, weapon.Item.Weight, currentUsageItem, exraLinearSpeed, doesAttackerHaveMount);
                    return false;
                }
                exraLinearSpeed *= 1f;
                float swingSpeed = (float)weapon.GetModifiedSwingSpeedForCurrentUsage() / 4.5454545f * num;

                if (weapon.Item != null && weapon.CurrentUsageItem != null)
                {
                    Agent attacker = null;
                    foreach (Agent agent in Mission.Current.Agents)
                    {
                        if (attackInformation.AttackerAgentOrigin == agent.Origin)
                        {
                            attacker = agent;
                        }
                    }
                    SkillObject skill = weapon.CurrentUsageItem.RelevantSkill;
                    int ef = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(attackerAgentCharacter, attackInformation.AttackerAgentOrigin, attacker.Formation, skill);
                    float effectiveSkillDR = Utilities.GetEffectiveSkillWithDR(ef);
                    switch (weapon.CurrentUsageItem.WeaponClass)
                    {
                        case WeaponClass.LowGripPolearm:
                        case WeaponClass.Mace:
                        case WeaponClass.OneHandedAxe:
                        case WeaponClass.OneHandedPolearm:
                        case WeaponClass.TwoHandedMace:
                            {
                                float swingskillModifier = 1f + (effectiveSkillDR / 1000f);
                                swingSpeed = swingSpeed * 0.83f * swingskillModifier * num;
                                break;
                            }
                        case WeaponClass.TwoHandedPolearm:
                            {
                                float swingskillModifier = 1f + (effectiveSkillDR / 1000f);

                                swingSpeed = swingSpeed * 0.83f * swingskillModifier * num;
                                break;
                            }
                        case WeaponClass.TwoHandedAxe:
                            {
                                float swingskillModifier = 1f + (effectiveSkillDR / 800f);

                                swingSpeed = swingSpeed * 0.75f * swingskillModifier * num;
                                break;
                            }
                        case WeaponClass.OneHandedSword:
                        case WeaponClass.Dagger:
                        case WeaponClass.TwoHandedSword:
                            {
                                float swingskillModifier = 1f + (effectiveSkillDR / 800f);

                                swingSpeed = swingSpeed * 0.9f * swingskillModifier * num;
                                break;
                            }
                    }
                }

                float num2 = MBMath.ClampFloat(0.4f / currentUsageItem.GetRealWeaponLength(), 0f, 1f);
                float num3 = MathF.Min(0.93f, impactPointAsPercent);
                float num4 = MathF.Min(0.93f, impactPointAsPercent + num2);
                //float originalValue = 0f;
                float newValue = 0f;
                int j = 0;
                //for (int i = 0; i < 5; i++)
                //{
                //    //float bladeLength = weapon.Item.WeaponDesign.UsedPieces[0].ScaledBladeLength;
                //    float impactPointAsPercent2 = num3 + (float)i / 4f * (num4 - num3);
                //    float num6 = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateStrikeMagnitudeForSwing(attackerAgentCharacter, attackerCaptainCharacter, swingSpeed, impactPointAsPercent2, weapon.Item.Weight, currentUsageItem, currentUsageItem.GetRealWeaponLength(), currentUsageItem.Inertia, currentUsageItem.CenterOfMass, exraLinearSpeed, doesAttackerHaveMount);
                //    if (originalValue < num6)
                //    {
                //        originalValue = num6;
                //    }
                //}

                float impactPointAsPercent3 = num3 + (float)0 / 4f * (num4 - num3);
                newValue = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateStrikeMagnitudeForSwing(attackerAgentCharacter, attackerCaptainCharacter, swingSpeed, impactPointAsPercent3, weapon.Item.Weight, currentUsageItem, currentUsageItem.GetRealWeaponLength(), currentUsageItem.Inertia, currentUsageItem.CenterOfMass, exraLinearSpeed, doesAttackerHaveMount);

                __result = newValue;
                return false;
            }
        }

        //[HarmonyPatch(typeof(CombatStatCalculator))]
        //[HarmonyPatch("CalculateStrikeMagnitudeForSwing")]
        //public class CalculateStrikeMagnitudeForSwingPatch
        //{
        //    public static bool Prefix(ref float __result, float swingSpeed, float impactPointAsPercent, float weaponWeight, float weaponLength, float weaponInertia, float weaponCoM, float extraLinearSpeed)
        //    {
        //        float distanceFromCoM = weaponLength * impactPointAsPercent - weaponCoM;
        //        float num2 = swingSpeed * (0.5f + weaponCoM) + extraLinearSpeed;
        //        float kineticEnergy = 0.5f * weaponWeight * num2 * num2;
        //        float inertiaEnergy = 0.5f * weaponInertia * swingSpeed * swingSpeed;
        //        float num5 = kineticEnergy + inertiaEnergy;
        //        float num6 = (num2 + swingSpeed * distanceFromCoM) / (1f / weaponWeight + distanceFromCoM * distanceFromCoM / weaponInertia);
        //        float num7 = num2 - num6 / weaponWeight;
        //        float num8 = swingSpeed - num6 * distanceFromCoM / weaponInertia;
        //        float num9 = 0.5f * weaponWeight * num7 * num7;
        //        float num10 = 0.5f * weaponInertia * num8 * num8;
        //        float num11 = num9 + num10;
        //        float num12 = num5 - num11 + 0.5f;
        //        __result =  0.067f * num12;
        //        //InformationManager.DisplayMessage(new InformationMessage("energy " + num12, Color.FromUint(4289612505u)));
        //        return false;
        //    }
        //}

        public static float CalculateMissileMagnitude(WeaponClass weaponClass, float weaponWeight, float missileSpeed, float missileTotalDamage, float momentumRemaining, DamageTypes damageType)
        {
            float baseMagnitude = 0f;
            switch (weaponClass)
            {
                case WeaponClass.Boulder:
                case WeaponClass.Stone:
                    {
                        missileTotalDamage *= 0.01f;
                        break;
                    }
                case WeaponClass.ThrowingAxe:
                case WeaponClass.ThrowingKnife:
                case WeaponClass.Dagger:
                    {
                        missileSpeed -= 0f; //5f
                        break;
                    }
                case WeaponClass.Javelin:
                    {
                        missileSpeed -= Utilities.throwableCorrectionSpeed;
                        if (missileSpeed < 5.0f)
                        {
                            missileSpeed = 5f;
                        }
                        break;
                    }
                case WeaponClass.OneHandedPolearm:
                    {
                        missileSpeed -= Utilities.throwableCorrectionSpeed;
                        if (missileSpeed < 5.0f)
                        {
                            missileSpeed = 5f;
                        }
                        break;
                    }
                case WeaponClass.LowGripPolearm:
                    {
                        missileSpeed -= Utilities.throwableCorrectionSpeed;
                        if (missileSpeed < 5.0f)
                        {
                            missileSpeed = 5f;
                        }
                        break;
                    }
                case WeaponClass.Arrow:
                    {
                        missileTotalDamage -= 100f;
                        missileTotalDamage *= 0.01f;
                        break;
                    }
                case WeaponClass.Bolt:
                    {
                        missileTotalDamage -= 100f;
                        missileTotalDamage *= 0.01f;
                        break;
                    }
            }

            float physicalDamage = ((missileSpeed * missileSpeed) * (weaponWeight)) / 2;
            float momentumDamage = (missileSpeed * weaponWeight);
            switch (weaponClass)
            {
                case WeaponClass.Boulder:
                case WeaponClass.Stone:
                    {
                        physicalDamage = (missileSpeed * missileSpeed * (weaponWeight) * 0.5f);
                        break;
                    }
                case WeaponClass.ThrowingAxe:
                case WeaponClass.ThrowingKnife:
                case WeaponClass.Dagger:
                    {
                        missileSpeed -= 0f; //5f
                        break;
                    }
                case WeaponClass.Javelin:
                case WeaponClass.OneHandedPolearm:
                case WeaponClass.LowGripPolearm:
                    {
                        if (physicalDamage > (weaponWeight) * 300f)
                        {
                            physicalDamage = (weaponWeight) * 300f;
                        }
                        break;
                    }
                case WeaponClass.Arrow:
                    {
                        if (physicalDamage > (weaponWeight) * 2250f)
                        {
                            physicalDamage = (weaponWeight) * 2250f;
                        }
                        break;
                    }
                case WeaponClass.Bolt:
                    {
                        if (physicalDamage > (weaponWeight) * 2500f)
                        {
                            physicalDamage = (weaponWeight) * 2500f;
                        }
                        break;
                    }
            }
            
            baseMagnitude = physicalDamage * missileTotalDamage * momentumRemaining;

            if (weaponClass == WeaponClass.Javelin)
            {
                missileTotalDamage = 0f;
                //baseMagnitude = (physicalDamage * momentumRemaining + (missileTotalDamage * 0.5f)) * RBMConfig.RBMConfig.ThrustMagnitudeModifier;
                if (damageType == DamageTypes.Pierce)
                {
                    baseMagnitude = (physicalDamage * momentumRemaining) * RBMConfig.RBMConfig.ThrustMagnitudeModifier;
                }
                else if (damageType == DamageTypes.Cut)
                {
                    baseMagnitude = (physicalDamage * momentumRemaining);
                }
                else
                {
                    baseMagnitude = (physicalDamage * momentumRemaining) * 0.5f;
                }
            }

            if (weaponClass == WeaponClass.ThrowingAxe)
            {
                baseMagnitude = physicalDamage * momentumRemaining;
            }
            if (weaponClass == WeaponClass.ThrowingKnife ||
                weaponClass == WeaponClass.Dagger)
            {
                baseMagnitude = (physicalDamage * momentumRemaining) * RBMConfig.RBMConfig.ThrustMagnitudeModifier * 0.6f;
            }

            if (weaponClass == WeaponClass.OneHandedPolearm ||
                weaponClass == WeaponClass.LowGripPolearm)
            {
                baseMagnitude = (physicalDamage * momentumRemaining) * RBMConfig.RBMConfig.ThrustMagnitudeModifier;
            }
            if (weaponClass == WeaponClass.Arrow ||
                weaponClass == WeaponClass.Bolt)
            {
                if (damageType == DamageTypes.Cut || damageType == DamageTypes.Pierce)
                {
                    baseMagnitude = physicalDamage * missileTotalDamage * momentumRemaining;
                }
                else
                {
                    baseMagnitude = physicalDamage * missileTotalDamage * momentumRemaining; // momentum makes more sense for blunt attacks, maybe 500 damage is needed for sling projectiles
                }
            }
            return baseMagnitude;
        }

        [HarmonyPatch(typeof(MissionCombatMechanicsHelper))]
        [HarmonyPatch("ComputeBlowMagnitudeMissile")]
        class ComputeBlowMagnitudeMissilePacth
        {
            static bool Prefix(in AttackInformation attackInformation, in AttackCollisionData acd, in MissionWeapon weapon, float momentumRemaining, in Vec2 victimVelocity, out float baseMagnitude, out float specialMagnitude)
            {
                Vec3 missileVelocity = acd.MissileVelocity;

                float missileTotalDamage = acd.MissileTotalDamage;

                WeaponComponentData currentUsageItem = weapon.CurrentUsageItem;
                ItemObject weaponItem;
                if (weapon.AmmoWeapon.Item != null)
                {
                    weaponItem = weapon.AmmoWeapon.Item;
                }
                else
                {
                    weaponItem = weapon.Item;
                }

                float length;
                if (!attackInformation.IsVictimAgentNull)
                {
                    length = (victimVelocity.ToVec3() - missileVelocity).Length;
                }
                else
                {
                    length = missileVelocity.Length;
                }
                baseMagnitude = CalculateMissileMagnitude(weapon.CurrentUsageItem.WeaponClass, weaponItem.Weight, length, missileTotalDamage, momentumRemaining, (DamageTypes) acd.DamageType);
                specialMagnitude = baseMagnitude;

                return false;
            }
        }

        [HarmonyPatch(typeof(CombatStatCalculator))]
        [HarmonyPatch("CalculateStrikeMagnitudeForPassiveUsage")]
        class ChangeLanceDamage
        {
            static bool Prefix(float weaponWeight, float extraLinearSpeed, ref float __result)
            {
                __result = CalculateStrikeMagnitudeForThrust(0f, weaponWeight, extraLinearSpeed, isThrown: false);
                return false;
            }

            private static float CalculateStrikeMagnitudeForThrust(float thrustWeaponSpeed, float weaponWeight, float extraLinearSpeed, bool isThrown)
            {
                float num = extraLinearSpeed * 1f; // because cav in the game is roughly 50% faster than it should be
                float num2 = 0.5f * weaponWeight * num * num * RBMConfig.RBMConfig.ThrustMagnitudeModifier; // lances need to have 3 times more damage to be preferred over maces
                return num2;

            }
        }    

        [HarmonyPatch(typeof(CombatStatCalculator))]
        [HarmonyPatch("CalculateStrikeMagnitudeForThrust")]
        class CalculateStrikeMagnitudeForThrustPatch
        {
            static bool Prefix(float thrustWeaponSpeed, float weaponWeight, float extraLinearSpeed, bool isThrown, ref float __result)
            {
                float combinedSpeed = MBMath.ClampFloat(thrustWeaponSpeed, 4f, 6f) + extraLinearSpeed;
                if (combinedSpeed > 0f)
                {

                    float kineticEnergy = 0.5f * weaponWeight * combinedSpeed * combinedSpeed;
                    float mixedEnergy = 0.5f * (weaponWeight + 1.5f) * combinedSpeed * combinedSpeed;
                    float baselineEnergy = 0.5f * 8f * combinedSpeed * combinedSpeed;
                    //float basedamage = 0.5f * (weaponWeight + 4.5f) * combinedSpeed * combinedSpeed;

                    //float basedamage = 120f;
                    //if (mixedEnergy > 120f)
                    //{
                    //    basedamage = mixedEnergy;
                    //}


                    //float handBonus = 0.5f * (weaponWeight + 1.5f) * combinedSpeed * combinedSpeed;
                    //float handLimit = 120f;
                    //if (handBonus > handLimit)
                    //{
                    //    handBonus = handLimit;
                    //}
                    //float thrust = handBonus;
                    //if (kineticEnergy > handLimit)
                    //{
                    //    thrust = kineticEnergy;
                    //}
                    //else if (basedamage > 180f)
                    //{
                    //    basedamage = 180f;
                    //}
                    //float thrust = basedamage;
                    //if (kineticEnergy > basedamage)
                    //{
                    //    thrust = kineticEnergy;
                    //}

                    //if (thrust > 200f)
                    //{
                    //    thrust = 200f;
                    //}
                    float thrust = baselineEnergy;
                    __result = thrust * RBMConfig.RBMConfig.ThrustMagnitudeModifier;
                    return false;
                }
                __result = 0f;
                return false;
            }
        }

        public static float CalculateSweetSpotSwingMagnitude(EquipmentElement weapon, int weaponUsageIndex, int relevantSkill)
        {
            float progressEffect = 1f;
            float sweetSpotMagnitude = -1f;

            if (weapon.Item != null && weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex) != null)
            {
                float swingSpeed = (float)weapon.GetModifiedSwingSpeedForUsage(weaponUsageIndex) / 4.5454545f * progressEffect;

                int ef = relevantSkill;
                float effectiveSkillDR = Utilities.GetEffectiveSkillWithDR(ef);
                switch (weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).WeaponClass)
                {
                    case WeaponClass.LowGripPolearm:
                    case WeaponClass.Mace:
                    case WeaponClass.OneHandedAxe:
                    case WeaponClass.OneHandedPolearm:
                    case WeaponClass.TwoHandedMace:
                        {
                            float swingskillModifier = 1f + (effectiveSkillDR / 1000f);
                            swingSpeed = swingSpeed * 0.83f * swingskillModifier * progressEffect;
                            break;
                        }
                    case WeaponClass.TwoHandedPolearm:
                        {
                            float swingskillModifier = 1f + (effectiveSkillDR / 1000f);
                            swingSpeed = swingSpeed * 0.83f * swingskillModifier * progressEffect;
                            break;
                        }
                    case WeaponClass.TwoHandedAxe:
                        {
                            float swingskillModifier = 1f + (effectiveSkillDR / 800f);

                            swingSpeed = swingSpeed * 0.75f * swingskillModifier * progressEffect;
                            break;
                        }
                    case WeaponClass.OneHandedSword:
                    case WeaponClass.Dagger:
                    case WeaponClass.TwoHandedSword:
                        {
                            float swingskillModifier = 1f + (effectiveSkillDR / 800f);

                            swingSpeed = swingSpeed * 0.9f * swingskillModifier * progressEffect;
                            break;
                        }
                }

                for (float currentSpot = 0.93f; currentSpot > 0.35f; currentSpot -= 0.05f)
                {
                    float weaponWeight = weapon.Item.Weight;
                    float weaponInertia = weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).Inertia;
                    float weaponCOM = weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).CenterOfMass;
                    float currentSpotMagnitude = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateStrikeMagnitudeForSwing(Hero.MainHero.CharacterObject, null, swingSpeed, currentSpot, weaponWeight,
                        weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex),
                        weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).GetRealWeaponLength(), weaponInertia, weaponCOM, 0f, false);
                    if (currentSpotMagnitude > sweetSpotMagnitude)
                    {
                        sweetSpotMagnitude = currentSpotMagnitude;
                    }
                }
            }
            return sweetSpotMagnitude;
        }

        public static float CalculateThrustMagnitude(EquipmentElement weapon, int weaponUsageIndex, int relevantSkill)
        {
            float progressEffect = 1f;
            float thrustMagnitude = -1f;

            if (weapon.Item != null && weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex) != null)
            {
                float thrustWeaponSpeed = (float)weapon.GetModifiedThrustSpeedForUsage(weaponUsageIndex) / 11.7647057f * progressEffect;

                int ef = relevantSkill;
                float effectiveSkillDR = Utilities.GetEffectiveSkillWithDR(ef);

                float weaponWeight = weapon.Item.Weight;
                float weaponInertia = weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).Inertia;
                float weaponCOM = weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).CenterOfMass;

                switch (weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).WeaponClass)
                {
                    case WeaponClass.LowGripPolearm:
                    case WeaponClass.Mace:
                    case WeaponClass.OneHandedAxe:
                    case WeaponClass.OneHandedPolearm:
                    case WeaponClass.TwoHandedMace:
                        {
                            float thrustskillModifier = 1f + (effectiveSkillDR / 1000f);

                            thrustWeaponSpeed = Utilities.CalculateThrustSpeed(weaponWeight, weaponInertia, weaponCOM);
                            thrustWeaponSpeed = thrustWeaponSpeed * 0.75f * thrustskillModifier * progressEffect;
                            break;
                        }
                    case WeaponClass.TwoHandedPolearm:
                        {
                            float thrustskillModifier = 1f + (effectiveSkillDR / 1000f);

                            thrustWeaponSpeed = Utilities.CalculateThrustSpeed(weaponWeight, weaponInertia, weaponCOM);
                            thrustWeaponSpeed = thrustWeaponSpeed * 0.7f * thrustskillModifier * progressEffect;
                            break;
                        }
                    case WeaponClass.TwoHandedAxe:
                        {
                            float thrustskillModifier = 1f + (effectiveSkillDR / 1000f);

                            thrustWeaponSpeed = Utilities.CalculateThrustSpeed(weaponWeight, weaponInertia, weaponCOM);
                            thrustWeaponSpeed = thrustWeaponSpeed * 0.9f * thrustskillModifier * progressEffect;
                            break;
                        }
                    case WeaponClass.OneHandedSword:
                    case WeaponClass.Dagger:
                    case WeaponClass.TwoHandedSword:
                        {
                            float thrustskillModifier = 1f + (effectiveSkillDR / 800f);

                            thrustWeaponSpeed = Utilities.CalculateThrustSpeed(weaponWeight, weaponInertia, weaponCOM);
                            thrustWeaponSpeed = thrustWeaponSpeed * 0.7f * thrustskillModifier * progressEffect;
                            break;
                        }
                }

                switch (weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).WeaponClass)
                {
                    case WeaponClass.OneHandedPolearm:
                    case WeaponClass.OneHandedSword:
                    case WeaponClass.Dagger:
                    case WeaponClass.Mace:
                        {
                            thrustMagnitude = Utilities.CalculateThrustMagnitudeForOneHandedWeapon(weaponWeight, effectiveSkillDR, thrustWeaponSpeed, 0f, Agent.UsageDirection.AttackDown);
                            break;
                        }
                    case WeaponClass.TwoHandedPolearm:
                    case WeaponClass.TwoHandedSword:
                        {
                            thrustMagnitude = Utilities.CalculateThrustMagnitudeForTwoHandedWeapon(weaponWeight, effectiveSkillDR, thrustWeaponSpeed, 0f, Agent.UsageDirection.AttackDown);
                            break;
                        }
                    default:
                        {
                            thrustMagnitude = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateStrikeMagnitudeForThrust(Hero.MainHero.CharacterObject, null, thrustWeaponSpeed, weaponWeight, weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex), 0f, false);
                            break;
                        }
                }
            }
            return thrustMagnitude;
        }

        public static void CalculateVisualSpeeds(EquipmentElement weapon, int weaponUsageIndex, float effectiveSkillDR, out float swingSpeedReal, out float thrustSpeedReal)
        {
            swingSpeedReal = 0f;
            thrustSpeedReal = 0f;
            if(!weapon.IsEmpty && weapon.Item != null && weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex) != null)
            {
                switch (weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).WeaponClass)
                {
                    case WeaponClass.LowGripPolearm:
                    case WeaponClass.Mace:
                    case WeaponClass.OneHandedAxe:
                    case WeaponClass.OneHandedPolearm:
                    case WeaponClass.TwoHandedMace:
                        {
                            float swingskillModifier = 1f + (effectiveSkillDR / 1000f);
                            float thrustskillModifier = 1f + (effectiveSkillDR / 1000f);

                            swingSpeedReal = MathF.Ceiling((weapon.GetModifiedSwingSpeedForUsage(weaponUsageIndex) * 0.83f) * swingskillModifier);
                            thrustSpeedReal = MathF.Floor(Utilities.CalculateThrustSpeed(weapon.Weight, weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).Inertia, weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).CenterOfMass) * 11.7647057f);
                            thrustSpeedReal = MathF.Ceiling((thrustSpeedReal * 1.1f) * thrustskillModifier);
                            break;
                        }
                    case WeaponClass.TwoHandedPolearm:
                        {
                            float swingskillModifier = 1f + (effectiveSkillDR / 1000f);
                            float thrustskillModifier = 1f + (effectiveSkillDR / 1000f);

                            swingSpeedReal = MathF.Ceiling((weapon.GetModifiedSwingSpeedForUsage(weaponUsageIndex) * swingskillModifier));
                            thrustSpeedReal = MathF.Floor(Utilities.CalculateThrustSpeed(weapon.Weight, weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).Inertia, weapon.Item.GetWeaponWithUsageIndex(weaponUsageIndex).CenterOfMass) * 11.7647057f);
                            thrustSpeedReal = MathF.Ceiling((thrustSpeedReal * 1.05f) * thrustskillModifier);
                            break;
                        }
                    case WeaponClass.TwoHandedAxe:
                        {
                            float swingskillModifier = 1f + (effectiveSkillDR / 800f);
                            float thrustskillModifier = 1f + (effectiveSkillDR / 1000f);

                            swingSpeedReal = MathF.Ceiling((weapon.GetModifiedSwingSpeedForUsage(weaponUsageIndex) * 0.75f) * swingskillModifier);
                            thrustSpeedReal = MathF.Ceiling((weapon.GetModifiedThrustSpeedForUsage(weaponUsageIndex) * 0.9f) * thrustskillModifier);
                            break;
                        }
                    case WeaponClass.OneHandedSword:
                    case WeaponClass.Dagger:
                    case WeaponClass.TwoHandedSword:
                        {
                            float swingskillModifier = 1f + (effectiveSkillDR / 800f);
                            float thrustskillModifier = 1f + (effectiveSkillDR / 800f);

                            swingSpeedReal = MathF.Ceiling((weapon.GetModifiedSwingSpeedForUsage(weaponUsageIndex) * 0.9f) * swingskillModifier);
                            thrustSpeedReal = MathF.Ceiling((weapon.GetModifiedThrustSpeedForUsage(weaponUsageIndex) * 1.15f) * thrustskillModifier);
                            break;
                        }
                }
            }

            swingSpeedReal /= 4.5454545f;
            thrustSpeedReal /= 11.7647057f;
        }

        [HarmonyPatch(typeof(ItemMenuVM))]
        [HarmonyPatch("SetWeaponComponentTooltip")]
        class SetWeaponComponentTooltipPatch
        {
            static void Postfix(ref ItemMenuVM __instance, in EquipmentElement targetWeapon, int targetWeaponUsageIndex, EquipmentElement comparedWeapon, int comparedWeaponUsageIndex, bool isInit)
            {
                MethodInfo methodAddFloatProperty = typeof(ItemMenuVM).GetMethod("AddFloatProperty", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(TextObject), typeof(float), typeof(float?), typeof(bool) }, null);
                methodAddFloatProperty.DeclaringType.GetMethod("AddFloatProperty", new[] { typeof(TextObject), typeof(float), typeof(float?), typeof(bool) });

                MethodInfo methodAddIntProperty = typeof(ItemMenuVM).GetMethod("AddIntProperty", BindingFlags.NonPublic | BindingFlags.Instance);
                methodAddIntProperty.DeclaringType.GetMethod("AddIntProperty");

                MethodInfo methodCreateProperty = typeof(ItemMenuVM).GetMethod("CreateProperty", BindingFlags.NonPublic | BindingFlags.Instance);
                methodCreateProperty.DeclaringType.GetMethod("CreateProperty");
                if (!targetWeapon.IsEmpty && targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex) != null && targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).IsRangedWeapon)
                {
                    if (Hero.MainHero != null)
                    {
                        Hero mainAgent = Hero.MainHero;
                        SkillObject skill = targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).RelevantSkill;
                        int effectiveSkill = mainAgent.GetSkillValue(skill);
                        float effectiveSkillDR = Utilities.GetEffectiveSkillWithDR(effectiveSkill);
                        float skillModifier = Utilities.CalculateSkillModifier(effectiveSkill);
                        int drawWeight = targetWeapon.GetModifiedMissileSpeedForUsage(targetWeaponUsageIndex);
                        float ammoWeightIdeal = MathF.Clamp(drawWeight / 1000f, 0f, 0.125f);

                        int calculatedMissileSpeed = Utilities.calculateMissileSpeed(ammoWeightIdeal, targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).ItemUsage, drawWeight);
                        if(targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).WeaponClass == WeaponClass.Bow)
                        {
                            methodCreateProperty.Invoke(__instance, new object[] { __instance.TargetItemProperties, "RBM Stats", "", 1, null });

                            methodAddIntProperty.Invoke(__instance, new object[] { new TextObject("Ideal Ammo Weight in grams: "), MathF.Round(ammoWeightIdeal * 1000f), MathF.Round(ammoWeightIdeal * 1000f) });
                            methodAddIntProperty.Invoke(__instance, new object[] { new TextObject("Initial Missile Speed: "), calculatedMissileSpeed, calculatedMissileSpeed });

                            //pierceArrows
                            bool shouldBreakNextTime = false;
                            float missileMagnitude = CalculateMissileMagnitude(WeaponClass.Arrow, ammoWeightIdeal, calculatedMissileSpeed, targetWeapon.GetModifiedThrustDamageForUsage(targetWeaponUsageIndex) + 100f, 1f, DamageTypes.Pierce);
                            string combinedDamageString = "A-Armor\nD-Full Damage\nP-Penetrated Damage\nB-Blunt Focre Trauma\n";
                            methodCreateProperty.Invoke(__instance, new object[] { __instance.TargetItemProperties, "", "Missile Damage Pierce", 1, null });
                            for (float i = 0; i <= 100; i += 10)
                            {
                                if (shouldBreakNextTime)
                                {
                                    //break;
                                }
                                int realDamage = MBMath.ClampInt(MathF.Floor(Utilities.RBMComputeDamage(WeaponClass.Arrow.ToString(),
                                DamageTypes.Pierce, missileMagnitude, i, 1f, out float penetratedDamage, out float bluntForce, 1f, null, false)), 0, 2000);
                                realDamage = MathF.Floor(realDamage * 1f);

                                if (penetratedDamage == 0f)
                                {
                                    shouldBreakNextTime = true;
                                }
                                combinedDamageString += "A: " + String.Format("{0,3}", i) + " D: " + String.Format("{0,-5}", realDamage) + " P: " + String.Format("{0,-5}", MathF.Floor(penetratedDamage)) + " B: " + MathF.Floor(bluntForce) + "\n";
                                //methodAddIntProperty.Invoke(__instance, new object[] { new TextObject("Thrust Damage " + i + " Armor: "), realDamage, realDamage });
                            }
                            __instance.TargetItemProperties[__instance.TargetItemProperties.Count - 1].PropertyHint = new HintViewModel(new TextObject(combinedDamageString));

                            //cut arrows
                            shouldBreakNextTime = false;
                            missileMagnitude = CalculateMissileMagnitude(WeaponClass.Arrow, ammoWeightIdeal, calculatedMissileSpeed, targetWeapon.GetModifiedThrustDamageForUsage(targetWeaponUsageIndex) + 115f, 1f, DamageTypes.Cut);
                            combinedDamageString = "A-Armor\nD-Full Damage\nP-Penetrated Damage\nB-Blunt Focre Trauma\n";
                            methodCreateProperty.Invoke(__instance, new object[] { __instance.TargetItemProperties, "", "Missile Damage Cut", 1, null });
                            for (float i = 0; i <= 100; i += 10)
                            {
                                if (shouldBreakNextTime)
                                {
                                    //break;
                                }
                                int realDamage = MBMath.ClampInt(MathF.Floor(Utilities.RBMComputeDamage(WeaponClass.Arrow.ToString(),
                                DamageTypes.Cut, missileMagnitude, i, 1f, out float penetratedDamage, out float bluntForce, 1f, null, false)), 0, 2000);
                                realDamage = MathF.Floor(realDamage * 1f);

                                if (penetratedDamage == 0f)
                                {
                                    shouldBreakNextTime = true;
                                }
                                combinedDamageString += "A: " + String.Format("{0,3}", i) + " D: " + String.Format("{0,-5}", realDamage) + " P: " + String.Format("{0,-5}", MathF.Floor(penetratedDamage)) + " B: " + MathF.Floor(bluntForce) + "\n";
                                //methodAddIntProperty.Invoke(__instance, new object[] { new TextObject("Thrust Damage " + i + " Armor: "), realDamage, realDamage });
                            }
                            __instance.TargetItemProperties[__instance.TargetItemProperties.Count - 1].PropertyHint = new HintViewModel(new TextObject(combinedDamageString));

                        }

                    }
                }
                if (!targetWeapon.IsEmpty && targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex) != null && targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).IsMeleeWeapon)
                {
                    if (Hero.MainHero != null)
                    {
                        Hero mainAgent = Hero.MainHero;
                        SkillObject skill = targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).RelevantSkill;
                        int effectiveSkill = mainAgent.GetSkillValue(skill);
                        float effectiveSkillDR = Utilities.GetEffectiveSkillWithDR(effectiveSkill);
                        float skillModifier = Utilities.CalculateSkillModifier(effectiveSkill);

                        CalculateVisualSpeeds(targetWeapon, targetWeaponUsageIndex, effectiveSkill, out float swingSpeedReal, out float thrustSpeedReal);
                        CalculateVisualSpeeds(comparedWeapon, comparedWeaponUsageIndex, effectiveSkill, out float swingSpeedRealCompred, out float thrustSpeedRealCompared);
                        
                        methodCreateProperty.Invoke(__instance, new object[] { __instance.TargetItemProperties, "RBM Stats", "", 1, null });

                        methodAddIntProperty.Invoke(__instance, new object[] { new TextObject("Relevant Skill: "), effectiveSkill, effectiveSkill });

                        methodAddFloatProperty.Invoke(__instance, new object[] { new TextObject("Swing Speed m/s:"), swingSpeedReal, swingSpeedRealCompred, false });
                        methodAddFloatProperty.Invoke(__instance, new object[] { new TextObject("Thrust Speed m/s: "), thrustSpeedReal, thrustSpeedRealCompared, false });

                        if (targetWeapon.GetModifiedSwingDamageForUsage(targetWeaponUsageIndex) > 0f)
                        {
                            float sweetSpotMagnitude = CalculateSweetSpotSwingMagnitude(targetWeapon, targetWeaponUsageIndex, effectiveSkill);
                            float sweetSpotMagnitudeCompared = CalculateSweetSpotSwingMagnitude(comparedWeapon, comparedWeaponUsageIndex, effectiveSkill);

                            float skillBasedDamage = Utilities.GetSkillBasedDamage(sweetSpotMagnitude, false, targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).WeaponClass.ToString(),
                                targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).SwingDamageType, effectiveSkillDR, skillModifier, StrikeType.Swing, targetWeapon.Item.Weight);

                            float skillBasedDamageCompared = sweetSpotMagnitudeCompared > 0f ? Utilities.GetSkillBasedDamage(sweetSpotMagnitudeCompared, false, comparedWeapon.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex).WeaponClass.ToString(),
                                comparedWeapon.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex).SwingDamageType, effectiveSkillDR, skillModifier, StrikeType.Swing, comparedWeapon.Item.Weight) : -1f;

                            float weaponDamageFactor = (float)Math.Sqrt(targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).SwingDamageFactor);
                            float weaponDamageFactorCompared = sweetSpotMagnitudeCompared > 0f ? (float)Math.Sqrt(comparedWeapon.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex).SwingDamageFactor) : -1f;

                            bool shouldBreakNextTime = false;
                            methodCreateProperty.Invoke(__instance, new object[] { __instance.TargetItemProperties, "", "Swing Damage", 1, null });

                            string combinedDamageString = "A-Armor\nD-Full Damage\nP-Penetrated Damage\nB-Blunt Focre Trauma\n";
                            string combinedDamageComparedString = "A-Armor\nD-Full Damage\nP-Penetrated Damage\nB-Blunt Focre Trauma\n";
                            for (float i = 0; i <= 100; i += 10)
                            {
                                if (shouldBreakNextTime)
                                {
                                    //break;
                                }
                                if (sweetSpotMagnitudeCompared > 0f)
                                {
                                    int realDamage = MBMath.ClampInt(MathF.Floor(Utilities.RBMComputeDamage(targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).WeaponClass.ToString(),
                                        targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).SwingDamageType, skillBasedDamage, i, 1f, out float penetratedDamage, out float bluntForce, weaponDamageFactor, null, false)), 0, 2000);
                                    realDamage = MathF.Floor(realDamage * 1f);

                                    int realDamageCompared = MBMath.ClampInt(MathF.Floor(Utilities.RBMComputeDamage(comparedWeapon.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex).WeaponClass.ToString(),
                                        comparedWeapon.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex).SwingDamageType, skillBasedDamageCompared, i, 1f, out float penetratedDamageCompared, out float bluntForceCompared, weaponDamageFactorCompared, null, false)), 0, 2000);
                                    realDamageCompared = MathF.Floor(realDamageCompared * 1f);

                                    if (penetratedDamage == 0f && penetratedDamageCompared == 0f)
                                    {
                                        shouldBreakNextTime = true;
                                    }
                                    combinedDamageString += "A: " + String.Format("{0,3}", i) + " D: " + String.Format("{0,-5}", realDamage) + " P: " + String.Format("{0,-5}", MathF.Floor(penetratedDamage)) + " B: " + MathF.Floor(bluntForce) + "\n";
                                    combinedDamageComparedString += "A: " + String.Format("{0,3}", i) + " D: " + String.Format("{0,-5}", realDamageCompared) + " P: " + String.Format("{0,-5}", MathF.Floor(penetratedDamageCompared)) + " B: " + MathF.Floor(bluntForceCompared) + "\n";
                                    //methodAddIntProperty.Invoke(__instance, new object[] { new TextObject("Swing Damage " + i + " Armor: "), realDamage, realDamageCompared });
                                    //__instance.TargetItemProperties[__instance.TargetItemProperties.Count - 1].PropertyHint = new HintViewModel(new TextObject("Penetrated: "+ MathF.Round(penetratedDamage, 1) + " Blunt Force: " + MathF.Round(bluntForce, 1)));
                                }
                                else
                                {
                                    int realDamage = MBMath.ClampInt(MathF.Floor(Utilities.RBMComputeDamage(targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).WeaponClass.ToString(), targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).SwingDamageType, skillBasedDamage, i, 1f, out float penetratedDamage, out float bluntForce, weaponDamageFactor, null, false)), 0, 2000);
                                    realDamage = MathF.Floor(realDamage * 1f);

                                    if (penetratedDamage == 0f)
                                    {
                                        shouldBreakNextTime = true;
                                    }
                                    combinedDamageString += "A: " + String.Format("{0,3}", i) + " D: " + String.Format("{0,-5}", realDamage) + " P: " + String.Format("{0,-5}", MathF.Floor(penetratedDamage)) + " B: " + MathF.Floor(bluntForce) + "\n";
                                    //methodAddIntProperty.Invoke(__instance, new object[] { new TextObject("Swing Damage " + i + " Armor: "), realDamage, realDamage });
                                    //__instance.TargetItemProperties[__instance.TargetItemProperties.Count - 1].PropertyHint = new HintViewModel(new TextObject("Penetrated: " + MathF.Round(penetratedDamage, 1) + " Blunt Force: " + MathF.Round(bluntForce, 1)));
                                }
                            }
                            __instance.TargetItemProperties[__instance.TargetItemProperties.Count - 1].PropertyHint = new HintViewModel(new TextObject(combinedDamageString));
                            if (!comparedWeapon.IsEmpty)
                            {
                                methodCreateProperty.Invoke(__instance, new object[] { __instance.ComparedItemProperties, "", "Swing Damage", 1, null });
                                __instance.ComparedItemProperties[__instance.ComparedItemProperties.Count - 1].PropertyHint = new HintViewModel(new TextObject(combinedDamageComparedString));
                            }
                        }

                        if (targetWeapon.GetModifiedThrustDamageForUsage(targetWeaponUsageIndex) > 0f)
                        {

                            float thrustMagnitude = CalculateThrustMagnitude(targetWeapon, targetWeaponUsageIndex, effectiveSkill);
                            float thrustMagnitudeCompared = CalculateThrustMagnitude(comparedWeapon, comparedWeaponUsageIndex, effectiveSkill);

                            float skillBasedDamage = Utilities.GetSkillBasedDamage(thrustMagnitude, false, targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).WeaponClass.ToString(),
                                targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).ThrustDamageType, effectiveSkillDR, skillModifier, StrikeType.Thrust, targetWeapon.Item.Weight);

                            float skillBasedDamageCompared = thrustMagnitudeCompared > 0f ? Utilities.GetSkillBasedDamage(thrustMagnitudeCompared, false, comparedWeapon.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex).WeaponClass.ToString(),
                                comparedWeapon.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex).ThrustDamageType, effectiveSkillDR, skillModifier, StrikeType.Thrust, comparedWeapon.Item.Weight) : -1f;

                            float weaponDamageFactor = (float)Math.Sqrt(targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).ThrustDamageFactor);
                            float weaponDamageFactorCompared = thrustMagnitudeCompared > 0f ? (float)Math.Sqrt(comparedWeapon.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex).ThrustDamageFactor) : -1f;

                            bool shouldBreakNextTime = false;
                            methodCreateProperty.Invoke(__instance, new object[] { __instance.TargetItemProperties, "", "Thrust Damage", 1, null });

                            string combinedDamageString = "A-Armor\nD-Full Damage\nP-Penetrated Damage\nB-Blunt Focre Trauma\n";
                            string combinedDamageComparedString = "A-Armor\nD-Full Damage\nP-Penetrated Damage\nB-Blunt Focre Trauma\n";
                            for (float i = 0; i <= 100; i += 10)
                            {
                                if (shouldBreakNextTime)
                                {
                                    //break;
                                }
                                if (thrustMagnitudeCompared > 0f)
                                {
                                    int realDamage = MBMath.ClampInt(MathF.Floor(Utilities.RBMComputeDamage(targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).WeaponClass.ToString(),
                                    targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).ThrustDamageType, skillBasedDamage, i, 1f, out float penetratedDamage, out float bluntForce, weaponDamageFactor, null, false)), 0, 2000);
                                    realDamage = MathF.Floor(realDamage * 1f);

                                    int realDamageCompared = MBMath.ClampInt(MathF.Floor(Utilities.RBMComputeDamage(comparedWeapon.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex).WeaponClass.ToString(),
                                    comparedWeapon.Item.GetWeaponWithUsageIndex(comparedWeaponUsageIndex).ThrustDamageType, skillBasedDamageCompared, i, 1f, out float penetratedDamageCompared, out float bluntForceCompared, weaponDamageFactorCompared, null, false)), 0, 2000);
                                    realDamageCompared = MathF.Floor(realDamageCompared * 1f);

                                    if (penetratedDamage == 0f && penetratedDamageCompared == 0f)
                                    {
                                        shouldBreakNextTime = true;
                                    }

                                    //methodAddIntProperty.Invoke(__instance, new object[] { new TextObject("Thrust Damage " + i + " Armor: "), realDamage, realDamageCompared });
                                    combinedDamageString += "A: " + String.Format("{0,3}", i) + " D: " + String.Format("{0,-5}", realDamage) + " P: " + String.Format("{0,-5}", MathF.Floor(penetratedDamage)) + " B: " + MathF.Floor(bluntForce) + "\n";
                                    combinedDamageComparedString += "A: " + String.Format("{0,3}", i) + " D: " + String.Format("{0,-5}", realDamageCompared) + " P: " + String.Format("{0,-5}", MathF.Floor(penetratedDamageCompared)) + " B: " + MathF.Floor(bluntForceCompared) + "\n";
                                }
                                else
                                {
                                    int realDamage = MBMath.ClampInt(MathF.Floor(Utilities.RBMComputeDamage(targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).WeaponClass.ToString(),
                                    targetWeapon.Item.GetWeaponWithUsageIndex(targetWeaponUsageIndex).ThrustDamageType, skillBasedDamage, i, 1f, out float penetratedDamage, out float bluntForce, weaponDamageFactor, null, false)), 0, 2000);
                                    realDamage = MathF.Floor(realDamage * 1f);

                                    if (penetratedDamage == 0f)
                                    {
                                        shouldBreakNextTime = true;
                                    }
                                    combinedDamageString += "A: " + String.Format("{0,3}", i) + " D: " + String.Format("{0,-5}", realDamage) + " P: " + String.Format("{0,-5}", MathF.Floor(penetratedDamage)) + " B: " + MathF.Floor(bluntForce) + "\n";
                                    //methodAddIntProperty.Invoke(__instance, new object[] { new TextObject("Thrust Damage " + i + " Armor: "), realDamage, realDamage });
                                }
                            }
                            __instance.TargetItemProperties[__instance.TargetItemProperties.Count - 1].PropertyHint = new HintViewModel(new TextObject(combinedDamageString));
                            if (!comparedWeapon.IsEmpty)
                            {
                                methodCreateProperty.Invoke(__instance, new object[] { __instance.ComparedItemProperties, "", "Thrust Damage", 1, null });
                                __instance.ComparedItemProperties[__instance.ComparedItemProperties.Count - 1].PropertyHint = new HintViewModel(new TextObject(combinedDamageComparedString));
                            }
                        }
                    }
                }
            }
        }
    }
}
