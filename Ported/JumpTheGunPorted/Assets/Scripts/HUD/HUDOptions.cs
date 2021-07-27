using UnityEngine;

public class HUDOptions : MonoBehaviour
{
	[Header("Children")]
	public HUDSlider _TerrainWidth;
	public HUDSlider _TerrainLength;
	
	public HUDSlider _MinTerrainHeight;
	public HUDSlider _MaxTerrainHeight;
	public HUDSlider _HeightDamage;
	
	public HUDSlider _TankCount;
	public HUDSlider _TankReloadTime;
	
	private void Start()
	{
		_TerrainWidth.SetBounds(1, 100);
		_TerrainWidth.Value = Config.Instance.Data.TerrainWidth;

		_TerrainLength.SetBounds(1, 100);
		_TerrainLength.Value = Config.Instance.Data.TerrainLength;

		_MinTerrainHeight.SetBounds(1, 10);
		_MinTerrainHeight.Value = Config.Instance.Data.MinTerrainHeight;

		_MaxTerrainHeight.SetBounds(1, 10);
		_MaxTerrainHeight.Value = Config.Instance.Data.MaxTerrainHeight;

		_HeightDamage.SetBounds(0, 10);
		_HeightDamage.Value = Config.Instance.Data.HeightDamage;

		_TankCount.SetBounds(0, 1000);
		_TankCount.Value = Config.Instance.Data.TankCount;

		_TankReloadTime.SetBounds(.1f, 20);
		_TankReloadTime.Value = Config.Instance.Data.TankReloadTime;
	}
	
	private void Update()
	{
		UpdateSliders();
	}

	private void UpdateSliders()
	{
		UpdateSlider(out Config.Instance.Data.TerrainWidth, _TerrainWidth, "Terrain Width");
		UpdateSlider(out Config.Instance.Data.TerrainLength, _TerrainLength, "Terrain Length");
		
		UpdateSlider(out Config.Instance.Data.MinTerrainHeight, _MinTerrainHeight, "Min Terrain Height");
		UpdateSlider(out Config.Instance.Data.MaxTerrainHeight, _MaxTerrainHeight, "Max Terrain Height");
		UpdateSlider(out Config.Instance.Data.HeightDamage, _HeightDamage, "Height Damage");
		
		UpdateSlider(out Config.Instance.Data.TankCount, _TankCount, "Tank Count");
		UpdateSlider(out Config.Instance.Data.TankReloadTime, _TankReloadTime, "Tank Reload Time");
	}

	private static void UpdateSlider(out int configValue, HUDSlider slider, string label)
	{
		configValue = Mathf.RoundToInt(slider.Value);
		slider.SetText(label + ": " + configValue);
	}

	private static void UpdateSlider(out float configValue, HUDSlider slider, string label)
	{
		configValue = slider.Value;
		slider.SetText(label + ": " + configValue);
	}
	
	public void OnRestart()
	{
		// TODO: Create Spawner entity here
	}
}
