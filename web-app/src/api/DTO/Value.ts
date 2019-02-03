export interface Value<T = null> {
	Type: number, 
	Payload: T | null
}

export function getInitialValue<T>() {
	return {
		Type: 0,
		Payload: null
	} as Value<T>;
}