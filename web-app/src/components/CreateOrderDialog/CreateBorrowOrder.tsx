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
	AllPeriod
}

interface IPaymentFreqView {
	name: string
	freq: PaymentFreq
}

enum SuretyType {
	Voucher,
	RealState,
	PTS
}

let suretyTypeLabels = new Map<SuretyType, string>();
suretyTypeLabels.set(SuretyType.Voucher, "Расписка");
suretyTypeLabels.set(SuretyType.RealState, "Недвижимость");
suretyTypeLabels.set(SuretyType.PTS, "ПТС");

interface ISurety {
	types: Map<SuretyType, boolean>
	others: string | null
}

enum CreditType {
	Auto,
	Business,
	Consumer,
	Hypothec,
	Micro,
	Refinancing,
	Other
}

let creditTypeLabels = new Map<CreditType, string>();
creditTypeLabels.set(CreditType.Consumer, "Потребительский");
creditTypeLabels.set(CreditType.Auto, "Автокредит");
creditTypeLabels.set(CreditType.Business, "Бизнес");
creditTypeLabels.set(CreditType.Hypothec, "Ипотека");
creditTypeLabels.set(CreditType.Micro, "Микро");
creditTypeLabels.set(CreditType.Refinancing, "Рефинансирование");
creditTypeLabels.set(CreditType.Other, "Другое");


let paymentsFreq: IPaymentFreqView[] = [
	{name:"Год", freq: PaymentFreq.Year},
	{name:"Месяц", freq: PaymentFreq.Month},
	{name:"День", freq: PaymentFreq.Day},
	{name:"В конце", freq: PaymentFreq.AllPeriod}
]

let initialSurety: ISurety = {
	types: new Map<SuretyType, boolean>(),
	others: null
}

initialSurety.types.set(SuretyType.Voucher, false);
initialSurety.types.set(SuretyType.RealState, false);
initialSurety.types.set(SuretyType.PTS, false);

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
			<div className="gi-create-order-delimiter"></div>
			{this.AmountInput()}
			{this.LocationInput()}
			{this.TermInput()}
			{this.PercentInput()}
			{this.PaymentFrequencyInput()}
			{this.SuretyInput()}
			{this.CreditTypeInput()}
			{this.CommentInput()}
			{this.SaveButton()}
			<div className="gi-create-order-delimiter"></div>
		</div>);
	}
	
	private AmountInput() {
		return (<div className="gi-create-order-input-row">
			{this.getLeftField("Сумма (₽)")}
			<div className="gi-right-field">
				<InputText 
					className="gi-input-text"
					keyfilter="pint" 
					onChange={(e) => this.setState({amount: parseInt(e.currentTarget.value)})}
					placeholder="₽"
					/>
			</div>
		</div>);
	}
	
	private LocationInput() {
		return (<div className="gi-create-order-input-row">
			{this.getLeftField("Местоположение")}
			<div className="gi-right-field">
				<LocationEdit 
					className="gi-input-text"
					intialCity="" 
					intialRegion="" 
					onChangeLocation={(c,r) => {}}
					placeholder="Местоположение" />
			</div>
		</div>);
	}
	
	private TermInput() {
		return (<div className="gi-create-order-input-row">
			<div className="w3-text-blue-grey gi-left-field">
				<span style={{float:"left"}}>Срок:</span>
				<span style={{float:"right", paddingRight:10}}>Лет:</span>
			</div>
			<div className="gi-right-field">
				<Spinner 
					inputClassName={"gi-date-spinner " + (this.state.years == 0 ? "gi-empty" : "")}
					max={80}
					min={0} 
					onChange={e => this.updateTermByYears(e.value)}
					value={this.state.years} />
				<span className="w3-text-blue-grey gi-date-tooltip" style={{width:"29.7333"}}>Мес:</span>
				<Spinner 
					inputClassName={"gi-date-spinner " + (this.state.months == 0 ? "gi-empty" : "")}
					max={80 * 12}
					min={0}
					value={this.state.months} 
					onChange={e => this.updateTermByMonths(e.value)} />
				<span className="w3-text-blue-grey gi-date-tooltip" style={{width:"36.7333"}}>Дней:</span>
				<Spinner 
					inputClassName={"gi-date-spinner " + (this.state.days == 0 ? "gi-empty" : "")}
					onChange={e => this.updateTermByDays(e.value)} 
					max={80 * 365}
					min={0}
					value={this.state.days} />
			</div>
		</div>);
	}
	
	private updateTermByYears(value: number) {
		this.setState({years: Math.min(value, 80)});
	}
	
	private updateTermByMonths(value: number) {
		let years = this.state.years;
		years += Math.floor(value / 12);
		value %= 12;
		years = Math.min(years, 80);
		this.setState({years, months: value});
	}
	
	private updateTermByDays(value: number) {
		let years = this.state.years;
		let months = this.state.months;
		years += Math.floor(value / 365);
		value = value % 365;
		months += Math.floor(value / 30);
		years += Math.floor(months / 12);
		months %= 12;
		value %= 30;
		years = Math.min(years, 80);
		this.setState({years, months, days: value});
	}
	
	private PercentInput() {
		return (<div className="gi-create-order-input-row">
			{this.getLeftField("Процент (%)")}
			<div className="gi-right-field">
				<Spinner 
					inputClassName={"gi-percent-spinner " + (this.state.percent == 0 ? "gi-empty" : "")}
					max={365}
					min={0} 
					onChange={e => this.setState({percent: e.value})}
					step={0.1}
					value={this.state.percent} />
			</div>
		</div>);
	}
	
	private PaymentFrequencyInput() {
		return (<div className="gi-create-order-input-row">
			{this.getLeftField("Частота выплат")}
			<div className="gi-right-field">
				<SelectButton 
					onChange={e => this.setState({paymentFreq: e.value})}
					optionLabel="name" 
					options={paymentsFreq}
					value={this.state.paymentFreq} />
			</div>
		</div>)
	}
	
	private SuretyInput() {
		let {surety} = this.state;
		let keyIndex = 0;
		let getCheckBox = (type: SuretyType, desc: string) => {
			let checked = surety.types.get(type);
			let result = (<div key={type} className="gi-surety gi-surety-cb">
				<Checkbox 
					inputId={"surety_" + type} 
					checked={checked}
					onChange={e => {surety.types.set(type, e.checked); this.forceUpdate();}} />
				<label htmlFor={"surety_" + type} className="p-checkbox-label">{desc}</label>
			</div>);
			return result;
		}
		let getAllCheckBoxesButtons = (labels: Map<SuretyType, string>) => {
			let nodes:JSX.Element[] = [];
			labels.forEach((v, k) => {
				nodes.push(getCheckBox(k, v));
			})
			return nodes;
		}
		return (<div className="gi-create-order-input-row">
			{this.getLeftField("Поручительство")}
			<div className="gi-right-field">
				{getAllCheckBoxesButtons(suretyTypeLabels)}
				<div className="gi-surety gi-surety-other">
					<InputText 
						className="gi-input-text"
						maxLength={30}
						onChange={(e) => this.setState({surety: {...surety, others: e.currentTarget.value}})}
						placeholder="Другое"
						value={surety.others != null ? surety.others : undefined} />
				</div>
			</div>
		</div>)
	}
	
	private CreditTypeInput() {
		let {creditType} = this.state;
		let getRadioButton = (type: CreditType, desc: string) => {
			return (<div key={type}  className="gi-credit-type-cb">
				<RadioButton 
					inputId={"credit_type_" + type}
					name="creditType" 
					onChange={e => this.setState({creditType: type})} 
					checked={creditType === type} />
				<label htmlFor={"credit_type_" + type} className="p-checkbox-label">{desc}</label>	
			</div>);
		}
		let getAllRadioButtons = (labels: Map<CreditType, string>) => {
			let nodes:JSX.Element[] = [];
			labels.forEach((v, k) => {
				nodes.push(getRadioButton(k, v));
			})
			return nodes;
		}
		return (<div className="gi-create-order-input-row">
			{this.getLeftField("Тип кредита")}
			<div className="gi-right-field">
				{getAllRadioButtons(creditTypeLabels)}
			</div>
		</div>);
	}
	
	private CommentInput() {
		let {comment} = this.state;
		return (<div className="gi-create-order-input-row">
			{this.getLeftField("Комментарий")}
			<div className="gi-right-field">
				<InputTextarea 
					autoResize={true}
					className="gi-input-text"
					onChange={e =>  this.setState({comment: e.currentTarget.value})}
					placeholder={"Комментарий"}
					rows={3} 
					value={comment || undefined}/>
			</div>
		</div>);
	}
	
	private getLeftField(name: string) {
		return (<div className="w3-text-blue-grey gi-left-field">
			{name + ":"}
		</div>)
	}
	
	private SaveButton() {
		return (<div className="gi-save-button-container">
			<Button 
				className="gi-save-button"
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