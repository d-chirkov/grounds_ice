import React, { CSSProperties } from "react";
import { Dispatch } from "redux";
import { connect } from "react-redux";
import { IRootState } from "../contexts/model";
import { SetRegistrationFormVisibleAction } from "../contexts/ui/registrationForm/actions";
import { SetAccountAction } from "../contexts/data/account/actions";
import { Toolbar } from "primereact/toolbar"; 
import { Button } from "primereact/button";
import { Dialog } from "primereact/dialog";

interface IHeaderMenuState {
	isLoggedIn: boolean,
	userName: string | null,
	registrationFormVisible: boolean,
}

interface IHeaderMenuDispatch {
	showRegistrationForm: () => void,
	hideRegistrationForm: () => void,
	registerAccount: () => void,
}

interface IHeaderMenuProps extends IHeaderMenuState, IHeaderMenuDispatch {
}

let HeaderMenu = (props: IHeaderMenuProps) => {
	let { isLoggedIn, userName, showRegistrationForm, hideRegistrationForm, registrationFormVisible, registerAccount } = props;
	let userNameStyle: CSSProperties = {
		marginTop: "7px",
		color: "white"
	}
	const registrationDialogFooter = (
		<div>
			<Button label="Да" icon="pi pi-check" onClick={() => {registerAccount(); hideRegistrationForm(); }} />
			<Button label="Нет" icon="pi pi-times" onClick={hideRegistrationForm} />
		</div>
	);
	return (<div>
		<Toolbar style={{height: "50px", backgroundColor: "#293851", borderRadius: "0px"}}>
			<div className="p-toolbar-group-right">
				{!isLoggedIn ? 
					<Button label="Регистрация" onClick={ showRegistrationForm }/> : 
					<p style={userNameStyle}>{userName}</p>
				}
			</div>
		</Toolbar>
		<Dialog 
			header="Регистрация" 
			visible={ registrationFormVisible } 
			style={ {width: '50vw'} } 
			modal={ true } 
			onHide={ hideRegistrationForm }
			footer={ registrationDialogFooter }
			>
			Вы согласны продать душу дьяволу?
		</Dialog>
	</div>)
}

let mapStateToProps = (state: IRootState): IHeaderMenuState => ({
	isLoggedIn: state.data.account != null,
	userName: state.data.account != null ? state.data.account.userName : null,
	registrationFormVisible: state.ui.registrationForm.visible,
})

let mapDispatchToProps = (dispatch: Dispatch): IHeaderMenuDispatch => ({
	showRegistrationForm: () => dispatch(new SetRegistrationFormVisibleAction(true)),
	hideRegistrationForm: () => dispatch(new SetRegistrationFormVisibleAction(false)),
	registerAccount: () => dispatch(new SetAccountAction("test_token", "userId", "test_name"))
})

export default connect(mapStateToProps, mapDispatchToProps)(HeaderMenu);
