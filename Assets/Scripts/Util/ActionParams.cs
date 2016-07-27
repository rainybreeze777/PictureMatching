using System;
using System.Collections.Generic;

public class ActionParams {

	private List<object> paramList = new List<object>();

	public int Count() {
		return paramList.Count;
	}

	public void AddToParamList(object param) {
		paramList.Add(param);
	}

	public Type GetParamType(int paramIndex) {
		return paramList[paramIndex].GetType();
	}

	public object GetArg(int paramIndex) {
		return paramList[paramIndex];
	}
}
