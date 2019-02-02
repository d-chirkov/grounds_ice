import React from "react";
import { connect } from "react-redux";
import { IRootState } from "../../../../contexts/model";
import { InputText } from "primereact/inputtext";
import { Button } from "primereact/button";

interface ILoginEditViewMapProps {
}

interface ILoginEditViewMapDispatch {
}

interface ILoginEditViewProps extends ILoginEditViewMapProps, ILoginEditViewMapDispatch {
}

interface ILoginEditViewState {
	loginInput: string
}

class LoginEditView extends React.Component<ILoginEditViewProps, ILoginEditViewState> {
	constructor(props: ILoginEditViewProps) {
		super(props);
	}
	
	render() {
		return (<div>
			<h4>Изменить логин</h4>
			<div className="w3-container">
				<InputText 
						type="text" 
						size={30} 
						onChange={(e) => { this.setState({loginInput: e.currentTarget.value}) }} 
						placeholder="Новый логин"
						/>
				<Button style={{float:"right"}} label="Сохранить" icon="pi pi-save" iconPos="left" onClick={() => {}} />
			</div>
		</div>);
	}
}


let mapStateToProps = (state: IRootState): ILoginEditViewMapProps => ({
})

let mapDispatchToProps = (dispatch: any): ILoginEditViewMapDispatch => ({
})

let LoginEditViewHOC = connect(mapStateToProps, mapDispatchToProps)(LoginEditView);

export { LoginEditViewHOC as LoginEditView };