import React from "react";
import { connect } from "react-redux";
import { HashRouter, Route, Switch, RouteComponentProps, withRouter } from "react-router-dom";

import { IRootState } from "../store/contexts/model";
import { UnsetAccountAction } from "../store/contexts/data/account/actions";
import { LogInFormShowAction } from "../store/contexts/ui/logInForm/actions";
import { CreateOrderFormShowAction } from "../store/contexts/ui/createOrderForm/actions";

import { Toolbar } from "primereact/toolbar";
import { Button } from "primereact/button";
import { Growl } from "primereact/growl";
import { TieredMenu } from "primereact/tieredmenu";
import { Messager } from "../Messager";

import { LogInDialog } from "./LogInDialog/LogInDialog";
import { CreateOrderDialog } from "./CreateOrderDialog/CreateOrderDialog";



interface IHeaderMenuMapProps {
	isLoggedIn: boolean,
	login: string | null,
	userId: string | null,
}

interface IHeaderMenuDispatchMapProps {
	showLogInDialog: () => void
	showCreateOrderDialog: () => void
	logOut: () => void
}

interface IHeaderMenuProps extends IHeaderMenuMapProps, IHeaderMenuDispatchMapProps, RouteComponentProps {
}

let HeaderMenu = (props: IHeaderMenuProps) => {
	let { isLoggedIn, login, userId, showLogInDialog, showCreateOrderDialog, logOut, history } = props;
	let popupHeaderMenu: TieredMenu | null = null;
	let popupHeaderMenuModel = [
		{ label: "Профиль", command: (e:any) => { popupHeaderMenu!.toggle(e); history.push(`/profile/id${userId}`); } }, 
		{ separator: true },
		{ label: "Выйти", command: (e:any) => {popupHeaderMenu!.toggle(e); history.push(`/`); logOut();} }];
	return (<div>
		<Toolbar style={{ backgroundColor: "#293851", borderRadius: "0px", border:0 }}>
			<div className="p-toolbar-group-left">
				{isLoggedIn && <Button className="w3-amber" label="Создать предложение" onClick={ showCreateOrderDialog } />}
			</div>
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
		
		{/* Popup Dialogs */}
		<LogInDialog />
		<CreateOrderDialog />
	</div>)
}

let mapStateToProps = (state: IRootState): IHeaderMenuMapProps => ({
	isLoggedIn: state.data.account != null,
	login: state.data.account != null ? state.data.account.login : null,
	userId: state.data.account != null ? state.data.account.userId : null,
})

let mapDispatchToProps = (dispatch: any): IHeaderMenuDispatchMapProps => ({
	showLogInDialog: () => dispatch(new LogInFormShowAction(true)),
	showCreateOrderDialog: () => dispatch(new CreateOrderFormShowAction(true)),
	logOut:() => dispatch(new UnsetAccountAction())
})

let HeaderMenuHOC = withRouter(connect(mapStateToProps, mapDispatchToProps)(HeaderMenu));

export { HeaderMenuHOC as HeaderMenu };
