import React, {Component} from "react";
import {Modal, Button, Row, Col, Form} from 'react-bootstrap';
import {PlaceTypeahead} from './PlaceTypeahead';
import { ToastContainer, toast} from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import moment from 'moment';

export class AddPersonModal extends Component {
    constructor(props){
        super(props);
        this.handleSubmit=this.handleSubmit.bind(this);
    }
    state = {
        place: null
    }

    handleChangePlaceIdValue=event=>this.setState({
        place:event.selected.length>0?
        {placeId:event.selected[0].placeId,
        placeName:event.selected[0].placeName}:null});

    handleSubmit(event){
        event.preventDefault();
        fetch(process.env.REACT_APP_API+'person',{
            method:'POST',
            headers:{
                'Accept':'application/json',
                'Content-Type':'application/json'
            },
            body:JSON.stringify({
                PersonId:this.props.personid,
                Name:event.target.PersonName.value,
                EventType:event.target.EventType.value,
                Birthdate:event.target.Birthdate.value,
                NextStart:event.target.NextStart.value,
                SunPos:"360",
                MoonPos:"360",
                VenusPos:"360",
                MarsPos:"360",
                JupiterPos:"360",
                SaturnPos:"360",
                NeptunePos:"360",
                EventPredictability:event.target.EventPredictability.value,
                PlaceId:this.state.place.placeId,
                Place:
                {
                    placeid:this.state.place.placeId,
                    placeName: this.state.place.placeName,
                    latitude:"N00.00.00",
                    longitude:"E000.00.00",
                    Persons: []
                }
            })
        })
        .then(res=>res.json())
        .then((result)=>{
            toast.success(result.eventPredictability +
                " ; " + result.venusPos +
                " ; " + moment(new Date()).add(result.uranusPos, 'minute').format('HH:mm'));
            this.props.onHide(event.target.Birthdate.value);
        },
        (error)=>{
            alert('failed');
        })
    }
    clickClose(event){
        this.props.onHide('');
   }
    render(){
        return(
            <div className="container">
                <Modal
                {...this.props}
                size="lg"
                aria-labelledby="contained-modal-title-vcenter"
                centered>
                    <Modal.Header closeButton>
                        <Modal.Title id="contained-modal-title-vcenter">
                            Submit Person
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Row>
                            <Col sm={8}>
                                <Form onSubmit={this.handleSubmit}>
                                    <Form.Group controlId="PersonName">
                                        <Form.Label>Person Name</Form.Label>
                                        <Form.Control type="text" name="PersonName" required
                                         placeholder="PersonName"/>
                                    </Form.Group>
                                    <Form.Group controlId="EventType">
                                        <Form.Label>Event Type</Form.Label>
                                        <Form.Control as="select" >
                                            <option value="0">0</option>
                                            <option value="1">1</option>
                                            <option value="2">2</option>
                                        </Form.Control>
                                    </Form.Group>
                                    <Form.Group controlId="EventPredictability">
                                        <Form.Label>Predictability</Form.Label>
                                        <Form.Control type="number" name="EventPredictability" required
                                        defaultValue="0" />
                                    </Form.Group>
                                    <Form.Group controlId="Birthdate">
                                        <Form.Label>Birthdate</Form.Label>
                                        <Form.Control type="datetime-local"/>
                                    </Form.Group>
                                    <Form.Group controlId="NextStart">
                                        <Form.Label>Next Start</Form.Label>
                                        <Form.Control type="datetime-local"/>
                                    </Form.Group>
                                    <Form.Group controlId="PlaceId">
                                        <Form.Label>Place</Form.Label>
                                        <PlaceTypeahead
                                            onChangeValue={this.handleChangePlaceIdValue}/>
                                    </Form.Group>
                                    <Form.Group>
                                        <Button variant="primary" type="submit">
                                            Submit Person
                                        </Button>
                                    </Form.Group>
                                </Form>
                            </Col>
                        </Row>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="danger" onClick={(e) => {
                            this.clickClose(e);
                            }}>Close</Button>
                    </Modal.Footer>
                </Modal>
                <ToastContainer />
            </div>
        )
    }
}