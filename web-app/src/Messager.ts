export interface IMessage {
	message: string
}

export class Messager {
	static growl_: any
	
	static setGrowl(growl: any) {
		Messager.growl_ = growl;
	}
	
	static showManyErrors(messages: string[]) {
		if (Messager.growl_ != undefined) {
			Messager.growl_.show(messages.map(v => ({severity: 'error', summary: v})));
		}
	}
	
	static showError(message: string) {
		this.showManyErrors([message]);
	}
	
	static showManySuccess(messages: string[]) {
		if (Messager.growl_ != undefined) {
			Messager.growl_.show(messages.map(v => ({severity: 'success', summary: v})));
		}
	}
	
	static showSuccess(message: string) {
		this.showManySuccess([message]);
	}
}
