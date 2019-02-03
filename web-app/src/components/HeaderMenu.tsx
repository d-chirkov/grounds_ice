import React from "react";
import { connect } from "react-redux";
import { HashRouter, Route, Switch, RouteComponentProps, withRouter } from "react-router-dom";

import { IRootState } from "../store/contexts/model";
import { UnsetAccountAction } from "../store/contexts/data/account/actions";
import { LogInFormShowAction } from "../store/contexts/ui/logInForm/actions";
import { Toolbar } from "primereact/toolbar";
import { Button } from "primereact/button";
import { Growl } from "primereact/growl";
import { TieredMenu } from "primereact/tieredmenu";

import { LogInDialog } from "./LogInDialog/LogInDialog";

import { Messager } from "../Messager";

interface IHeaderMenuState {
	isLoggedIn: boolean,
	login: string | null,
	userId: string | null,
	isLogInDialogVisible: boolean,
}

interface IHeaderMenuDispatch {
	showLogInDialog: () => void,
	closeLogInDialog: () => void
	logOut: () => void
}

interface IHeaderMenuProps extends IHeaderMenuState, IHeaderMenuDispatch, RouteComponentProps {
}

let HeaderMenu = (props: IHeaderMenuProps) => {
	let { isLogInDialogVisible, isLoggedIn, login, userId, showLogInDialog, logOut, history } = props;
	let popupHeaderMenu: TieredMenu | null = null;
	let popupHeaderMenuModel = [
		{ label: "Профиль", command: (e:any) => { popupHeaderMenu!.toggle(e); history.push(`/profile/id${userId}`); } }, 
		{ separator: true },
		{ label: "Выйти", command: (e:any) => {popupHeaderMenu!.toggle(e); logOut();} }];
	return (<div>
		<Toolbar style={{ backgroundColor: "#293851", borderRadius: "0px" }}>
			<div className="p-toolbar-group-right">
				{!isLoggedIn ?
					<Button className="w3-amber" label="Вход и регистрация" onClick={ showLogInDialog } /> :
					<div>
						<TieredMenu style={{marginTop:"12px"}} model={popupHeaderMenuModel} popup={true} ref={el => popupHeaderMenu = el} />
						<Button label={login!} onClick={ e => popupHeaderMenu!.toggle(e) } />
					</div>
				}
			</div>
		</Toolbar>
		<Growl ref={(el) => Messager.setGrowl(el)} style={{marginTop:"50px"}} />
		{isLogInDialogVisible && <LogInDialog />}
		
	</div>)
}

let mapStateToProps = (state: IRootState): IHeaderMenuState => ({
	isLoggedIn: state.data.account != null,
	login: state.data.account != null ? state.data.account.login : null,
	userId: state.data.account != null ? state.data.account.userId : null,
	isLogInDialogVisible: state.ui.logInForm.visible,
})

let mapDispatchToProps = (dispatch: any): IHeaderMenuDispatch => ({
	showLogInDialog: () => dispatch(new LogInFormShowAction(true)),
	closeLogInDialog: () => dispatch(new LogInFormShowAction(false)),
	logOut:() => dispatch(new UnsetAccountAction())
})

let HeaderMenuHOC = withRouter(connect(mapStateToProps, mapDispatchToProps)(HeaderMenu));

export { HeaderMenuHOC as HeaderMenu };
