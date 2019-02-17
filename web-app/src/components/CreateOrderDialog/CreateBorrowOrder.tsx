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
import { Messager } from "../../Messager";
import { string } from "prop-types";

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

interface IErrorFlags {
	isAmmountError: boolean
	isLocationError: boolean	
}

let initialErrorFlags: IErrorFlags = {
	isAmmountError: false,
	isLocationError: false,
}

interface ICreateBorrowOrderState {
	amount: number
	region: string | null
	city: string | null
	years: number
	months: number
	days: number
	percent: number
	paymentFreq: IPaymentFreqView | null
	surety: ISurety
	creditType: CreditType
	comment: string | null
	errors: IErrorFlags
	sending: boolean
}

class CreateBorrowOrder extends React.Component<ICreateBorrowOrderProps, ICreateBorrowOrderState> {
	constructor(props: ICreateBorrowOrderProps) {
		super(props);
		this.state = {
			amount: 0,
			region: null,
			city: null,
			years: 0,
			months: 0,
			days: 0,
			percent: 0,
			paymentFreq: null,
			surety: initialSurety,
			creditType: CreditType.Consumer,
			comment: null,
			errors: initialErrorFlags,
			sending: false
		}
	}
	
	render() {
		return (<div>
			<div className="gi-create-order-delimiter"></div>
			{this.amountInput()}
			{this.locationInput()}
			{this.termInput()}
			{this.percentInput()}
			{this.paymentFrequencyInput()}
			{this.suretyInput()}
			{this.creditTypeInput()}
			{this.commentInput()}
			{this.saveButton()}
			<div className="gi-create-order-delimiter"></div>
		</div>);
	}
	
	private send() {
		if (!this.validateOrder()) {
			return;
		}
		this.setState({sending: true});
		//TODO: send 
	}
	
	private validateOrder(): boolean {
		let {amount, city, region} = this.state;
		let errors: string[] = [];
		let errorFlags: IErrorFlags = initialErrorFlags;
		if (amount < 1000) {
			errors.push("Сумма должна быть больше 1000₽");
			errorFlags.isAmmountError = true;
		}
		if (amount > 50000000) {
			errors.push("Сумма должна быть меньше 50.000.000₽");
			errorFlags.isAmmountError = true;
		}
		if (city == null && region == null) {
			errors.push("Укажите местоположение");
			errorFlags.isLocationError = true;
		}
		if (errors.length > 0) {
			this.setState({errors: errorFlags});
			Messager.showManyErrors(errors);
			return false;
		}
		return true;
	}
	
	private amountInput() {
		let {errors, sending} = this.state;
		return (<div className="gi-create-order-input-row">
			{this.getLeftField("Сумма (₽)")}
			<div className="gi-right-field">
				<InputText 
					className={"gi-input-text " + (errors.isAmmountError ? "p-error" : "")}
					disabled={sending}
					keyfilter="pint" 
					onChange={(e) => this.setState({
						errors: {...errors, isAmmountError: false}, 
						amount: parseInt(e.currentTarget.value)})}
					placeholder="₽"
					/>
			</div>
		</div>);
	}
	
	private locationInput() {
		let {errors, sending} = this.state;
		return (<div className="gi-create-order-input-row">
			{this.getLeftField("Местоположение")}
			<div className="gi-right-field">
				<LocationEdit 
					className={"gi-input-text " + (errors.isLocationError ? "p-error" : "")}
					disabled={sending}
					intialCity="" 
					intialRegion="" 
					onChangeLocation={(city, region) => this.setState({region, city})}
					onChange={() => this.setState({errors: {...errors, isLocationError: false}})}
					placeholder="Местоположение" />
			</div>
		</div>);
	}
	
	private termInput() {
		let {years, months, days, sending} = this.state;
		return (<div className="gi-create-order-input-row">
			<div className="w3-text-blue-grey gi-left-field">
				<span style={{float:"left"}}>Срок:</span>
				<span style={{float:"right", paddingRight:10}}>Лет:</span>
			</div>
			<div className="gi-right-field">
				<Spinner 
					disabled={sending}
					inputClassName={"gi-date-spinner " + (years == 0 ? "gi-empty" : "")}
					max={80}
					min={0} 
					onChange={e => this.updateTermByYears(e.value)}
					value={years} />
				<span className="w3-text-blue-grey gi-date-tooltip" style={{width:"29.7333"}}>Мес:</span>
				<Spinner 
					disabled={sending}
					inputClassName={"gi-date-spinner " + (months == 0 ? "gi-empty" : "")}
					max={80 * 12}
					min={0}
					value={months} 
					onChange={e => this.updateTermByMonths(e.value)} />
				<span className="w3-text-blue-grey gi-date-tooltip" style={{width:"36.7333"}}>Дней:</span>
				<Spinner 
					disabled={sending}
					inputClassName={"gi-date-spinner " + (days == 0 ? "gi-empty" : "")}
					onChange={e => this.updateTermByDays(e.value)} 
					max={80 * 365}
					min={0}
					value={days} />
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
	
	private percentInput() {
		let {percent, sending} = this.state;
		return (<div className="gi-create-order-input-row">
			{this.getLeftField("Процент (%)")}
			<div className="gi-right-field">
				<Spinner 
					disabled={sending}
					inputClassName={"gi-percent-spinner " + (percent == 0 ? "gi-empty" : "")}
					max={365}
					min={0} 
					onChange={e => this.setState({percent: e.value})}
					step={0.1}
					value={percent} />
			</div>
		</div>);
	}
	
	private paymentFrequencyInput() {
		let {paymentFreq, sending} = this.state;
		return (<div className="gi-create-order-input-row">
			{this.getLeftField("Частота выплат")}
			<div className="gi-right-field">
				<SelectButton 
					disabled={sending}
					onChange={e => this.setState({paymentFreq: e.value})}
					optionLabel="name" 
					options={paymentsFreq}
					value={paymentFreq} />
			</div>
		</div>)
	}
	
	private suretyInput() {
		let {surety, sending} = this.state;
		let getCheckBox = (type: SuretyType, desc: string) => {
			let checked = surety.types.get(type);
			let result = (<div key={type} className="gi-surety gi-surety-cb">
				<Checkbox 
					checked={checked}
					disabled={sending}
					inputId={"surety_" + type} 
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
						disabled={sending}
						maxLength={30}
						onChange={(e) => this.setState({surety: {...surety, others: e.currentTarget.value}})}
						placeholder="Другое"
						value={surety.others != null ? surety.others : undefined} />
				</div>
			</div>
		</div>)
	}
	
	private creditTypeInput() {
		let {creditType, sending} = this.state;
		let getRadioButton = (type: CreditType, desc: string) => {
			return (<div key={type}  className="gi-credit-type-cb">
				<RadioButton 
					checked={creditType === type} 
					disabled={sending}
					inputId={"credit_type_" + type}
					name="creditType" 
					onChange={e => this.setState({creditType: type})} />
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
	
	private commentInput() {
		let {comment, sending} = this.state;
		return (<div className="gi-create-order-input-row">
			{this.getLeftField("Комментарий")}
			<div className="gi-right-field">
				<InputTextarea 
					autoResize={true}
					className="gi-input-text"
					disabled={sending}
					onChange={e =>  this.setState({comment: e.currentTarget.value})}
					placeholder={"Комментарий"}
					rows={3} 
					maxLength={300}
					value={comment || undefined}/>
			</div>
		</div>);
	}
	
	private getLeftField(name: string) {
		return (<div className="w3-text-blue-grey gi-left-field">
			{name + ":"}
		</div>)
	}
	
	private saveButton() {
		let {sending} = this.state;
		return (<div className="gi-save-button-container">
			<Button 
				className="gi-save-button"
				disabled={sending}
				label="Создать"
				icon={sending ? "pi pi-spin pi-spinner" : "pi pi-save"}
				iconPos="left" 
				onClick={() => {this.send()}}/>
		</div>);
	}
}


let mapStateToProps = (state: IRootState): ICreateBorrowOrderMapProps => ({
})

let mapDispatchToProps = (dispatch: any): ICreateBorrowOrderMapDispatch => ({
})

let CreateBorrowOrderHOC = connect(mapStateToProps, mapDispatchToProps)(CreateBorrowOrder);

export { CreateBorrowOrderHOC as CreateBorrowOrder };