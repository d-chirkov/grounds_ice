import "./CreateBorrowOrder.css"

import React from "react";
import { connect } from "react-redux";
import { IRootState } from "../../store/contexts/model";

import { InputText } from "primereact/inputtext";
import { LocationEdit } from "../Shared/LocationEdit";
import { Spinner } from "primereact/spinner";
import { SelectButton } from "primereact/selectbutton";
import { Checkbox } from "primereact/checkbox";
import { RadioButton } from "primereact/radiobutton";
import { InputTextarea } from "primereact/inputtextarea";
import { Button } from "primereact/button";
import { Dropdown } from "primereact/dropdown";

interface ICreateBorrowOrderMapProps {
}

interface ICreateBorrowOrderMapDispatch {
}

interface ICreateBorrowOrderProps extends ICreateBorrowOrderMapProps, ICreateBorrowOrderMapDispatch {
}

// TODO: must be part of model
enum PaymentFreq {
	Year,
	Month,
	Day,
	AllPeriod,
	Custom
}

interface IPaymentFreqView {
	name: string
	freq: PaymentFreq
}

interface ISurety {
	voucher: boolean
	realEstate: boolean
	pts: boolean
	others: string | null
}

enum CreditType {
	Auto,
	Business,
	Consumer,
	Hypothec,
	Micro,
	Other
}

let creditTypeLabels = new Map<CreditType, string>();
creditTypeLabels.set(CreditType.Consumer, "Потребительский");
creditTypeLabels.set(CreditType.Auto, "Автокредит");
creditTypeLabels.set(CreditType.Business, "Бизнес");
creditTypeLabels.set(CreditType.Hypothec, "Ипотека");
creditTypeLabels.set(CreditType.Micro, "Микро");
creditTypeLabels.set(CreditType.Other, "Другое");


let rowHeight = "40px";
let rowLineHeight = "32px";
let leftWidth = 130;
let rightWidth = 290;
let totalWidth = leftWidth + rightWidth;
let wideInputWidth = 31;
let spinnersSize = 1;

let paymentsFreqPadding = 1.7;

//<span key="Год" style={{padding:paymentsFreqPadding}}>Год</span>

let paymentsFreq: IPaymentFreqView[] = [
	{name:"Год", freq: PaymentFreq.Year},
	{name:"Месяц", freq: PaymentFreq.Month},
	{name:"День", freq: PaymentFreq.Day},
	{name:"В конце", freq: PaymentFreq.AllPeriod}
]

let initialSurety: ISurety = {
	voucher: false,
	realEstate: false,
	pts: false,
	others: null
}


interface ICreateBorrowOrderState {
	amount: number
	years: number
	months: number
	days: number
	percent: number
	paymentFreq: IPaymentFreqView | null
	surety: ISurety
	creditType: CreditType
	comment: string | null
}

class CreateBorrowOrder extends React.Component<ICreateBorrowOrderProps, ICreateBorrowOrderState> {
	constructor(props: ICreateBorrowOrderProps) {
		super(props);
		this.state = {
			amount: 0,
			years: 0,
			months: 0,
			days: 0,
			percent: 0,
			paymentFreq: null,
			surety: initialSurety,
			creditType: CreditType.Consumer,
			comment: null
		}
	}
	
	render() {
		return (<div>
			<div style={{float: "left", width:totalWidth, height:20}}></div>
			{this.AmountInput()}
			{this.LocationInput()}
			{this.TermInput()}
			{this.PercentInput()}
			{this.PaymentFrequencyInput()}
			{this.SuretyInput()}
			{this.CreditTypeInput()}
			{this.CommentInput()}
			{this.SaveButton()}
			<div style={{float: "left", width:totalWidth, height:20}}></div>
		</div>);
	}
	
	private AmountInput() {
		return (<div className="create-order-input-row">
			<div className="w3-text-blue-grey" style={{lineHeight:rowLineHeight, width:leftWidth, float:"left", height:rowHeight}}>
				Сумма (₽):
			</div>
			<div style={{float: "left", height:rowHeight, width:rightWidth, wordWrap:"break-word"}}>
				<InputText 
					keyfilter="pint" 
					size={wideInputWidth} 
					placeholder="₽"
					onChange={(e) => this.setState({amount: parseInt(e.currentTarget.value)})}
					/>
			</div>
		</div>);
	}
	
	private LocationInput() {
		return (<div className="create-order-input-row">
			<div className="w3-text-blue-grey" style={{lineHeight:rowLineHeight, width:leftWidth, float:"left", height:rowHeight}}>
				<span>Местоположение:</span>
			</div>
			<div style={{float: "left", height:rowHeight, width:rightWidth, wordWrap:"break-word"}}>
				<LocationEdit 
					intialCity="" 
					intialRegion="" 
					placeholder="Местоположение" 
					size={wideInputWidth} 
					onChangeLocation={(c,r) => {}}/>
			</div>
		</div>);
	}
	
	private TermInput() {
		let tooltipPadding = 6.7;
		return (<div className="create-order-input-row">
			<div className="w3-text-blue-grey" style={{lineHeight:rowLineHeight, width:leftWidth, float:"left", height:rowHeight}}>
				<span style={{float:"left"}}>Срок:</span>
				<span style={{float:"right", paddingRight:10}}>Лет:</span>
			</div>
			<div style={{float: "left", height:rowHeight, width:rightWidth, wordWrap:"break-word"}}>
				<Spinner 
					inputStyle={this.state.years == 0 ? {color:"rgb(179, 179, 179)"} : undefined}
					size={spinnersSize} 
					value={this.state.years} 
					onChange={(e) => this.setState({years: e.value})}
					max={99}
					min={0} />
				<span className="w3-text-blue-grey" style={{padding:tooltipPadding}}>Мес:</span>
				<Spinner 
					inputStyle={this.state.months == 0 ? {color:"rgb(179, 179, 179)"} : undefined}
					size={spinnersSize} 
					value={this.state.months} 
					onChange={(e) => this.setState({months: e.value})}
					max={11}
					min={0} />
				<span className="w3-text-blue-grey" style={{padding:tooltipPadding}}>Дней:</span>
				<Spinner 
					inputStyle={this.state.days == 0 ? {color:"rgb(179, 179, 179)"} : undefined}
					size={spinnersSize} 
					value={this.state.days}
					onChange={(e) => this.setState({days: e.value})} 
					max={30}
					min={0} />
			</div>
		</div>);
	}
	
	private PercentInput() {
		return (<div className="create-order-input-row">
			<div className="w3-text-blue-grey" style={{lineHeight:rowLineHeight, width:leftWidth, float:"left", height:rowHeight}}>
				Процент (%):
			</div>
			<div style={{float: "left", height:"auto", width:rightWidth, wordWrap:"break-word"}}>
				<div style={{float: "left", height:rowHeight, width:rightWidth, wordWrap:"break-word"}}>
					<Spinner 
						size={spinnersSize} 
						value={this.state.percent} 
						onChange={(e) => this.setState({percent: e.value})}
						max={100}
						min={0} 
						step={0.1}/>
				</div>
			</div>
		</div>);
	}
	
	private PaymentFrequencyInput() {
		return (<div className="create-order-input-row">
			<div className="w3-text-blue-grey" style={{lineHeight:"30px", width:leftWidth, float:"left", height:"30px"}}>
				Частота выплат:
			</div>
			<div style={{float: "left", height:"auto", width:rightWidth, wordWrap:"break-word"}}>
				<SelectButton optionLabel="name" value={this.state.paymentFreq} options={paymentsFreq} onChange={e => this.setState({paymentFreq: e.value})} />
			</div>
		</div>)
	}
	
	private SuretyInput() {
		let {surety} = this.state;
		return (<div className="create-order-input-row">
			<div className="w3-text-blue-grey" style={{lineHeight:rowLineHeight, width:leftWidth, float:"left", height:rowHeight}}>
				Поручительство:
			</div>
			<div style={{paddingTop:4, float: "left", height:"auto", width:rightWidth}}>
				<div style={{ float: "left", width:rightWidth}}>
					<Checkbox 
						inputId="surety_vaucher_cb" 
						checked={surety.voucher}
						onChange={e => this.setState({surety: {...surety, voucher: e.checked}})} />
					<label htmlFor="surety_vaucher_cb" className="p-checkbox-label">Расписка</label>
				</div>
				<div style={{paddingTop:5, float: "left", width:rightWidth}}>
					<Checkbox 
						inputId="surety_real_estate_cb" 
						checked={surety.realEstate}
						onChange={e => this.setState({surety: {...surety, realEstate: e.checked}})} />
					<label htmlFor="surety_real_estate_cb" className="p-checkbox-label">Недвижимость</label>
				</div>
				<div style={{paddingTop:5, float: "left", width:rightWidth}}>
					<Checkbox 
						inputId="surety_pts_cb" 
						checked={surety.pts}
						onChange={e => this.setState({surety: {...surety, pts: e.checked}})} />
					<label htmlFor="surety_pts_cb" className="p-checkbox-label">ПТС</label>
				</div>
				<div style={{paddingTop:7, float: "left", width:rightWidth}}>
					<InputText 
						size={wideInputWidth} 
						value={surety.others != null ? surety.others : undefined}
						placeholder="Другое"
						maxLength={30}
						onChange={(e) => this.setState({surety: {...surety, others: e.currentTarget.value}})}
						/>
				</div>
			</div>
		</div>)
	}
	
	private CreditTypeInput() {
		let {creditType} = this.state;
		let getRadioButton = (type: CreditType, desc: string) => {
			return (<div style={{ float: "left", width:rightWidth}}>
				<RadioButton 
					inputId="credit_type_consumer_rb" 
					name="creditType" 
					onChange={e => this.setState({creditType: type})} 
					checked={creditType === type} />
				<label htmlFor="credit_type_consumer_rb" className="p-checkbox-label">{desc}</label>	
			</div>);
		}
		let getAllRadioButtons = (labels: Map<CreditType, string>) => {
			let nodes:JSX.Element[] = [];
			labels.forEach((v, k) => {
				nodes.push(getRadioButton(k, v));
			})
			return nodes;
		}
		let creditTypes = Array.from(creditTypeLabels.keys()).map((v) => ({name:creditTypeLabels.get(v), value:v}));
		return (<div className="create-order-input-row">
			<div className="w3-text-blue-grey" style={{lineHeight:rowLineHeight, width:leftWidth, float:"left", height:rowHeight}}>
				Тип кредита:
			</div>
			
			<div style={{paddingTop:4, float:"left", width:rightWidth}}>
				<Dropdown 
					optionLabel="name"
					value={{name:creditTypeLabels.get(creditType), value:creditType}} 
					options={creditTypes} 
					style={{width:283}}
					onChange={e => {this.setState({creditType: e.value.value})}} />
				{/* {getAllRadioButtons(creditTypeLabels)} */}
			</div>
		</div>);
	}
	
	private CommentInput() {
		let {comment} = this.state;
		return (<div className="create-order-input-row">
			<div className="w3-text-blue-grey" style={{lineHeight:"40px", width:leftWidth, float:"left", height:rowHeight}}>
				Комментарий:
			</div>
			<div style={{paddingTop:10, float: "left", height:"auto", width:rightWidth, paddingBottom:"14px"}}>
				<InputTextarea 
					rows={5} 
					cols={wideInputWidth} 
					autoResize={true}
					onChange={e =>  this.setState({comment: e.currentTarget.value})}
					placeholder={"Комментарий"}
					value={comment || undefined}/>
			</div>
		</div>);
	}
	
	private SaveButton() {
		return (<div style={{paddingTop:4, float: "left", height:"auto", width:totalWidth, paddingBottom:"14px"}}>
			<Button 
				style={{margin:"auto", float: "right", marginRight:6}} 
				label="Создать"
				iconPos="left" 
				disabled={false}
				onClick={() => {}}/>
		</div>);
	}
}


let mapStateToProps = (state: IRootState): ICreateBorrowOrderMapProps => ({
})

let mapDispatchToProps = (dispatch: any): ICreateBorrowOrderMapDispatch => ({
})

let CreateBorrowOrderHOC = connect(mapStateToProps, mapDispatchToProps)(CreateBorrowOrder);

export { CreateBorrowOrderHOC as CreateBorrowOrder };