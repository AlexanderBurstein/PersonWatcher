import React, {Component} from 'react';
import {Modal, Button, Row, Col, Form} from 'react-bootstrap';

export class EventImageModal extends Component<{ [key: string]: any}, { [key: string]: any}> {
    constructor(props: { [key: string]: any}){
        super(props);
        this.state = {
            img:""
        }
    }
  
    fetchImage = async () => {
        if (this.props.personId) {
            const res = await fetch(process.env.REACT_APP_API+'Person/Visuals?personId=' + this.props.personId);
            const imageBlob = await res.blob();
            const imageObjectURL = URL.createObjectURL(imageBlob);
            this.setState({img: imageObjectURL});
        }
    };
  
    clickClose(event:any){
        this.props.onHide(this.props.birthdate);
   }
    render() {
        return(
                <div className="container">
                    <Modal
                    {...this.props}
                size="xl"
                onEnter={this.fetchImage}
                aria-labelledby="contained-modal-title-vcenter"
                centered>
                    <Modal.Header closeButton>
                        <Modal.Title id="contained-modal-title-vcenter">
                            Event Details
                        </Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Row>
                            <Col>
                                <img src={this.state.img}/>
                            </Col>
                        </Row>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="danger" onClick={(e) => {
                            this.clickClose(e);
                            }}>Close</Button>
                    </Modal.Footer>
                </Modal>
            </div>
        )
    }
}