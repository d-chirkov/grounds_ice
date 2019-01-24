class Messager {
	static growl_: any
	
	setGrowl(growl: any) {
		Messager.growl_ = growl;
	}
	
	showWarning(header: string, message: string) {
		if (Messager.growl_ != undefined) {
			Messager.growl_.show({severity: 'error', summary: header, detail: message });
		}
	}
}

let messager: Messager = new Messager();

export default messager;