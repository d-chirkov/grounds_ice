import React from "react";
import { connect } from "react-redux";
import { IRootState } from "../../../../contexts/model";
import { Password } from "primereact/password";
import { Button } from "primereact/button";

interface IPasswordEditViewMapProps {
}

interface IPasswordEditViewMapDispatch {
}

interface IPasswordEditViewProps extends IPasswordEditViewMapProps, IPasswordEditViewMapDispatch {
}

interface IPasswordEditViewState {
}

class PasswordEditView extends React.Component<IPasswordEditViewProps, IPasswordEditViewState> {
	constructor(props: IPasswordEditViewProps) {
		super(props);
	}

	render() {
		return (<div>
			<h4>Изменить пароль</h4>
			<div className="w3-container" style={{paddingBottom:"14px"}}>
				<Password
					type="text"
					size={30}
					feedback={false}
					onChange={(e) => { this.setState({ loginInput: e.currentTarget.value }) }}
					placeholder="Старый пароль" />
			</div>
			<div className="w3-container" style={{paddingBottom:"14px"}}>
				<Password
					type="text"
					size={30}
					feedback={false}
					onChange={(e) => { this.setState({ loginInput: e.currentTarget.value }) }}
					placeholder="Новый пароль" />
			</div>
			<div className="w3-container">
				<Password
					type="text"
					size={30}
					feedback={false}
					onChange={(e) => { this.setState({ loginInput: e.currentTarget.value }) }}
					placeholder="Повторите новый пароль" />
				<Button style={{ float:"right" }} label="Сохранить" icon="pi pi-save" iconPos="left" onClick={() => { }} />
			</div>
		</div>);
	}
}


let mapStateToProps = (state: IRootState): IPasswordEditViewMapProps => ({
})

let mapDispatchToProps = (dispatch: any): IPasswordEditViewMapDispatch => ({
})

let PasswordEditViewHOC = connect(mapStateToProps, mapDispatchToProps)(PasswordEditView);

export { PasswordEditViewHOC as PasswordEditView };