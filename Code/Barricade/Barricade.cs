namespace _01_Works.CM._01_Scripts.Barricade
{
    public class Barricade : BuildObject
    {
        private void OnEnable()
        {
            OnSetUpAnimation += SetUpInstalled;
        }

        private void SetUpInstalled()
        {
            isInstalled = true;
            OnSetUpAnimation -= SetUpInstalled;
        }
    }
}
