import React, { CSSProperties } from "react";
import { connect } from "react-redux";
import { IRootState } from "../contexts/model";
import { UnsetAccountAction } from "../contexts/data/account/actions";
import { LogInFormShowAction } from "../contexts/ui/logInForm/actions";
import { Toolbar } from "primereact/toolbar";
import { Button } from "primereact/button";
import { Growl } from "primereact/growl";
import { TieredMenu } from "primereact/tieredmenu";

import LogInDialog from "./LogInDialog/LogInDialog";

import { Messager } from "../Messager";

interface IHeaderMenuState {
	isLoggedIn: boolean,
	userName: string | null,
	isLogInDialogVisible: boolean,
}

interface IHeaderMenuDispatch {
	showLogInDialog: () => void,
	closeLogInDialog: () => void
	logOut: () => void
}

interface IHeaderMenuProps extends IHeaderMenuState, IHeaderMenuDispatch {
}

let HeaderMenu = (props: IHeaderMenuProps) => {
	let { isLogInDialogVisible, isLoggedIn, userName, showLogInDialog, logOut } = props;
	let popupHeaderMenu: TieredMenu | null = null;
	let popupHeaderMenuModel = [
		{ label: "Профиль", command: (e:any) => popupHeaderMenu!.toggle(e) }, 
		{ separator: true },
		{ label: "Выйти", command: (e:any) => {popupHeaderMenu!.toggle(e); logOut();} }];
	return (<div>
		<Toolbar style={{ backgroundColor: "#293851", borderRadius: "0px" }}>
			<div className="p-toolbar-group-right">
				{!isLoggedIn ?
					<Button className="w3-amber" label="Вход и регистрация" onClick={ showLogInDialog } /> :
					<div>
						<TieredMenu style={{marginTop:"12px"}} model={popupHeaderMenuModel} popup={true} ref={el => popupHeaderMenu = el} />
						<Button label={userName!} onClick={ e => popupHeaderMenu!.toggle(e) } />
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
	userName: state.data.account != null ? state.data.account.username : null,
	isLogInDialogVisible: state.ui.logInForm.visible,
})

let mapDispatchToProps = (dispatch: any): IHeaderMenuDispatch => ({
	showLogInDialog: () => dispatch(new LogInFormShowAction(true)),
	closeLogInDialog: () => dispatch(new LogInFormShowAction(false)),
	logOut:() => dispatch(new UnsetAccountAction()),
})

export default connect(mapStateToProps, mapDispatchToProps)(HeaderMenu);
