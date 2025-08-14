using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour
{
    [Header("Pool (for SFX and bursts)")]
    [SerializeField] private AudioSourcePool _pool;

    [Header("Banks")]
    [SerializeField] private SfxBank _musicBank;
    [SerializeField] private SfxBank _ambienceBank;
    [SerializeField] private SfxBank _sfxBank;

    // Optional external hooks
    public UnityAction<SoundCategory, int> onMusic;
    public UnityAction<SoundCategory, int> onSFX;
    public UnityAction<SoundCategory, int> onAmbience;

    private readonly Dictionary<int, SfxEvent> _musicMap = new();
    private readonly Dictionary<int, SfxEvent> _ambienceMap = new();
    private readonly Dictionary<int, SfxEvent> _sfxMap = new();

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        // singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;


        BuildMap(_musicBank, _musicMap);
        BuildMap(_ambienceBank, _ambienceMap);
        BuildMap(_sfxBank, _sfxMap);

        // (optional) wire events
        onMusic = PlaySFX;
        onAmbience = PlaySFX;
        onSFX = PlaySFX;
    }

    private static void BuildMap(SfxBank bank, Dictionary<int, SfxEvent> map)
    {
        map.Clear();
        if (bank == null || bank.events == null) return;

        foreach (var ev in bank.events)
        {
            if (ev == null || ev.id == 0 && (ev.clips == null || ev.clips.Length == 0)) { /* allow id=0 too */ }
            if (!map.ContainsKey(ev.id) && ev.PickClip() != null)
                map.Add(ev.id, ev);
        }
    }

    public void PlaySFX(SoundCategory soundCategory,int id)
    {
        if (_pool == null) return;
        //call the event of each catagory
        Dictionary<int, SfxEvent> _map = //pick the map that matches the sound category
            soundCategory == SoundCategory.Music ? _musicMap :
            soundCategory == SoundCategory.Ambience ? _ambienceMap :
            _sfxMap;
        if (!TryPick(_map, id, out var ev, out var clip)) return;
            GetSoundSourceToPlay(ev);
    }

    // ---------- Helpers ----------
    private static bool TryPick(Dictionary<int, SfxEvent> map, int id, out SfxEvent ev, out AudioClip clip)
    {
        ev = null; clip = null;
        if (!map.TryGetValue(id, out ev)) return false;
        clip = ev.PickClip();
        return clip != null;
    }
    private void GetSoundSourceToPlay(SfxEvent sfxEvent)
    {
        if (_pool == null)
        {
            Debug.Log("<color=red>pool is null</color>");
            return;
        }
        var src = _pool.GetPooledObject();
        if (src == null) return;
        src.Source.loop = sfxEvent.loop;
        src.Source.clip = sfxEvent.PickClip();
        src.Source.Play();
    }
}
