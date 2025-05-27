namespace CMPM.Relics.Effects {
	public interface IRelicEffect {
		public void ApplyEffect();
		public void RevertEffect();
		
		public bool CanCancel();
	}
}