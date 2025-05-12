using CMPM.Utils.Structures;


namespace CMPM.Utils {
    public interface IRPNEvaluator {
        public SerializedDictionary<string, float> GetRPNVariables();
    }
}