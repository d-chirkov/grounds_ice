import "../../styles/PopupDialog.css"
import "./CreateOrderDialog.css"

import React from "react";
import { connect } from "react-redux";
import { IRootState } from "../../store/contexts/model"
import { CreateOrderFormShowAction } from "../../store/contexts/ui/createOrderForm/actions";

import { Dialog } from "primereact/dialog";
import { Button } from "primereact/button";
import { TabView, TabPanel } from "primereact/tabview";

interface ICreateOrderDialogMapProps {
	isDialogVisible: boolean
}

interface ICreateOrderDialogMapDispatch {
	closeDialog: () => void
}

interface ICreateOrderDialogProps extends ICreateOrderDialogMapProps, ICreateOrderDialogMapDispatch {
}

interface ICreateOrderDialogState {
	tabIndex: number
}

let initialCreateOrderDialogState:ICreateOrderDialogState = {
	tabIndex: 0
}

class CreateOrderDialog extends React.Component<ICreateOrderDialogProps, ICreateOrderDialogState> {
	constructor(props: ICreateOrderDialogProps) {
		super(props);
		this.state = initialCreateOrderDialogState;
	}
	
	shouldComponentUpdate(nextProps: ICreateOrderDialogProps): boolean {
		if (!this.props.isDialogVisible && !nextProps.isDialogVisible) {
			return false;
		}
		if (this.props.isDialogVisible != nextProps.isDialogVisible) {
			this.setState(initialCreateOrderDialogState);
		}
		return true;
	}

	render() {
		if (!this.props.isDialogVisible) {
			return null;
		}
		let dialogHeight = "40vw";
		return (<div>
			<Dialog
				visible={true}
				className="popup-dialog"
				style={{ height: dialogHeight }}
				contentStyle={{ border: "0px", height: dialogHeight, paddingLeft:0, paddingRight:0 }}
				showHeader={false}
				modal={true}
				blockScroll={true}
				onHide={() => this.props.closeDialog()}
			>
				<h2 style={{paddingLeft:20, paddingTop:0}} className="w3-opacity"><b>Создать предложение</b></h2>
				<Button
					id="popup-dialog-close-button"
					icon="pi pi-times"
					className="p-button-rounded p-button-secondary"
					style={{ position: "absolute", top: 0, right: 0 }}
					onClick={() => this.props.closeDialog()} />
				<TabView style={{padding:"0px"}} activeIndex={this.state.tabIndex} onTabChange={(e) => this.setState({ tabIndex: e.index })}>
					<TabPanel contentClassName="create-order-dialog-tab" contentStyle={{border:0}} headerStyle={{ paddingLeft:"10px"}} header="Займ">
						Создать займ
					</TabPanel>
					<TabPanel header="Кредит">
						Создать кредит
					</TabPanel>
				</TabView> 
			</Dialog>
		</div>);
	}
}


let mapStateToProps = (state: IRootState): ICreateOrderDialogMapProps => ({
	isDialogVisible: state.ui.createOrderForm.visible
})

let mapDispatchToProps = (dispatch: any): ICreateOrderDialogMapDispatch => ({
	closeDialog: () => dispatch(new CreateOrderFormShowAction(false))
})

let CreateOrderDialogHOC = connect(mapStateToProps, mapDispatchToProps)(CreateOrderDialog);

export { CreateOrderDialogHOC as CreateOrderDialog };