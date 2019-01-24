import React, { CSSProperties } from "react";
import { connect } from "react-redux";
import { IRootState } from "../contexts/model";
import { LogInFormShowAction } from "../contexts/ui/logInForm/actions";
import { SignUpAction, SignInAction } from "../contexts/data/account/actions";
import { Toolbar } from "primereact/toolbar";
import { Button } from "primereact/button";
import { Growl } from "primereact/growl";

import LogInDialog from "./LogInDialog/LogInDialog";

import Messager from "../Messager";

interface IHeaderMenuState {
	isLoggedIn: boolean,
	userName: string | null,
	isLogInDialogVisible: boolean,
}

interface IHeaderMenuDispatch {
	showLogInDialog: () => void,
	closeLogInDialog: () => void
}

interface IHeaderMenuProps extends IHeaderMenuState, IHeaderMenuDispatch {
}

let HeaderMenu = (props: IHeaderMenuProps) => {
	let { isLogInDialogVisible, isLoggedIn, userName, showLogInDialog, closeLogInDialog } = props;
	let userNameStyle: CSSProperties = {
		marginTop: "7px",
		color: "white"
	}
	return (<div>
		<Toolbar style={{ height: "50px", backgroundColor: "#293851", borderRadius: "0px" }}>
			<div className="p-toolbar-group-right">
				{!isLoggedIn ?
					<Button label="Вход и регистрация" onClick={ showLogInDialog } /> :
					<p style={userNameStyle}>{userName}</p>
				}
			</div>
		</Toolbar>
		<Growl ref={(el) => Messager.setGrowl(el)} />
		{isLogInDialogVisible && <LogInDialog />}
	</div>)
}

let mapStateToProps = (state: IRootState): IHeaderMenuState => ({
	isLoggedIn: state.data.account != null,
	userName: state.data.account != null ? state.data.account.username : null,
	isLogInDialogVisible: state.ui.logInForm.visible,
})

let mapDispatchToProps = (dispatch: any): IHeaderMenuDispatch => ({
	showLogInDialog: () => dispatch(new LogInFormShowAction(true)),
	closeLogInDialog: () => dispatch(new LogInFormShowAction(false))
})

export default connect(mapStateToProps, mapDispatchToProps)(HeaderMenu);
