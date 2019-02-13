import "../../styles/PopupDialog.css"
import "../../styles/ScrollPanel.css"
import "./CreateOrderDialog.css"

import React from "react";
import { connect } from "react-redux";
import { IRootState } from "../../store/contexts/model"
import { CreateOrderFormShowAction } from "../../store/contexts/ui/createOrderForm/actions";

import { CreateBorrowOrder } from "./CreateBorrowOrder";

import { Dialog } from "primereact/dialog";
import { Button } from "primereact/button";
import { TabView, TabPanel } from "primereact/tabview";
import { ScrollPanel } from "primereact/scrollpanel";
import ScrollLock, { TouchScrollable } from "react-scrolllock";

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
		return (<div className="gi-create-order-dialog">
			<Dialog
				id="create-order-dialog"
				visible={true}
				className="popup-dialog gi-dialog"
				contentStyle={{ border: "0px", paddingLeft:0, paddingRight:0, height:"40vw" }}
				showHeader={false}
				modal={true}
				blockScroll={true}
				onHide={() => this.props.closeDialog()}
			>
				<ScrollLock />
				<TouchScrollable>
					<h3 className="gi-header w3-text-blue-grey">СОЗДАТЬ ПРЕДЛОЖЕНИЕ</h3>
					<Button
						id="popup-dialog-close-button"
						icon="pi pi-times"
						className="p-button-rounded p-button-secondary"
						onClick={() => this.props.closeDialog()} />
					<TabView className="gi-tab-view" activeIndex={this.state.tabIndex} onTabChange={(e) => this.setState({ tabIndex: e.index })}>
						<TabPanel header="Займ" contentClassName="gi-tab-panel-content" headerClassName="gi-tab-panel-header">
							<ScrollPanel className="gi-scroll-panel gi-tab-scroll">
								<CreateBorrowOrder />
							</ScrollPanel>
						</TabPanel>
						<TabPanel header="Кредит">
							В разработке
						</TabPanel>
					</TabView> 
				</TouchScrollable>
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