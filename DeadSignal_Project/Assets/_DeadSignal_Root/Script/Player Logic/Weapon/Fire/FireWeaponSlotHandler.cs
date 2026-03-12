using System;
using UnityEngine;

/// <summary>
/// [FireWeaponSlotHandler]: Gestiona los slots de armas de fuego del jugador.
/// Permite almacenar armas en dos slots y controlar cuál está activo.
/// También expone eventos para notificar cambios a otros sistemas (UI, equipamiento, etc).
/// </summary>
public class FireWeaponSlotHandler : MonoBehaviour
{
    #region Slot Configuration
    public FireWeaponType slot1 = FireWeaponType.AssaultRifle; //Amra asignada al primer slot (Default: AssaultRifle) 
    public FireWeaponType slot2 = FireWeaponType.Empty; //Arma asignada al segundo slot
    #endregion

    #region Internal State
    int currentSlot = 0; //Índice del slot actualmente seleccionado. (0 = slot1 / 1 = slot2)
    #endregion

    #region Events
    //Evento invocado cuando el contenido de los slots cambia
    public event Action OnSlotsUpdated;
    //Evento invocado cando el jugador cambia el slot activo.
    public event Action<int> OnSlotChanged;
    #endregion

    #region Slot Queries
    /// <summary>
    /// Devuelve el tipo de arma actualmente equipada según el slot activo.
    /// </summary>
    public FireWeaponType GetCurrentWeapon()
    {
        return currentSlot == 0 ? slot1 : slot2;
    }

    /// <summary>
    /// Devuelve el índice del slot actualmente sleeccioando.
    /// </summary>
    public int GetCurrentSlot()
    {
        return currentSlot;
    }
    #endregion

    #region Slot Management
    /// <summary>
    /// Asigna un arma al primer slot disponible. 
    /// Si el primer slot está ocupado, se asigna al segundo.
    /// </summary>
    public void SetWeaponToSlot(FireWeaponType type)
    {
        if (slot1 == FireWeaponType.Empty)
            slot1 = type;
        else
            slot2 = type;

        OnSlotsUpdated?.Invoke();
    }

    /// <summary>
    /// Cambia el slot actualmente activo. 
    /// Si el slot ya está seleccionado, no se realiza ninguna acción. 
    /// </summary>
    public void SelectSlot(int slot)
    {
        if (currentSlot == slot) return;

        currentSlot = slot;
        OnSlotChanged?.Invoke(slot);
    }
    #endregion
}