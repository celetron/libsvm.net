﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace libsvm
{
    public abstract class SVM
    {
        protected svm_problem prob;
        protected svm_parameter param;
        protected svm_model model;


        /// <summary>
        /// Default SVM
        /// </summary>
        /// <remarks>The class store svm parameters and create the model. 
        /// This way, you can use it to predict</remarks>
        public SVM(svm_problem prob, int svm_type, int kernel_type, int degree,
            double C, double gamma, double coef0, double nu, double cache_size,
            double eps, double p, int shrinking, int probability, int nr_weight,
            int[] weight_label, double[] weight)
            : this(prob, new svm_parameter()
            {
                svm_type = svm_type,
                kernel_type = kernel_type,
                degree = degree,
                C = C,
                gamma = gamma,
                coef0 = coef0,
                nu = nu,
                cache_size = cache_size,
                eps = eps,
                p = p,
                shrinking = shrinking,
                probability = probability,
                nr_weight = nr_weight,
                weight_label = weight_label,
                weight = weight,
            })
        { }
        /// <summary>
        /// Default SVM
        /// </summary>
        /// <remarks>The class store svm parameters and create the model.
        /// This way, you can use it to predict</remarks>
        public SVM(svm_problem prob, int svm_type,
            Kernel kernel, double C,
            double nu, double cache_size,
            double eps, double p, int shrinking, int probability, int nr_weight,
            int[] weight_label, double[] weight)
            : this(prob, new svm_parameter()
            {
                svm_type = svm_type,
                kernel_type = (int) kernel.KernelType,
                degree = kernel.Degree,
                C = C,
                gamma = kernel.Gamma,
                coef0 = kernel.R,
                nu = nu,
                cache_size = cache_size,
                eps = eps,
                p = p,
                shrinking = shrinking,
                probability = probability,
                nr_weight = nr_weight,
                weight_label = weight_label,
                weight = weight,
            })
        { }
        /// <summary>
        /// Default SVM
        /// </summary>
        /// <remarks>The class store svm parameters and create the model.
        /// This way, you can use it to predict</remarks>
        public SVM(svm_problem prob, svm_parameter param)
        {
            var error = svm.svm_check_parameter(prob, param);
            if (error != null)
            {
                throw new Exception(error);
            }

            this.prob = prob;
            this.param = param;

            this.Train();
        }

        /// <summary>
        /// Default SVM
        /// </summary>
        /// <remarks>The class imports the model directly.
        /// This way, you can use it to predict</remarks>
        public SVM(string model_file_name)
        {
            this.Import(model_file_name);
        }

        /// <summary>
        /// Default SVM
        /// </summary>
        /// <remarks>The class expects to import the model directly using the LoadXML method.
        /// This way, you can use it to predict</remarks>
        public SVM()
        {
        }

        /// <summary>
        /// Default SVM
        /// </summary>
        /// <remarks>The class store svm parameters and create the model.
        /// This way, you can use it to predict</remarks>
        public SVM(string input_file_name, svm_parameter param)
            : this(ProblemHelper.ReadProblem(input_file_name), param)
        {
        }

        /// <summary>
        /// Train the SVM and save the model
        /// </summary>
        public void Train()
        {
            this.model = svm.svm_train(prob, param);
        }

        /// <summary>
        /// Export the model in an xml file
        /// </summary>
        public void Export(string model_file_name)
        {
            if (this.model == null)
                throw new Exception("No trained svm model");

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(svm_model));
                TextWriter writer = new StreamWriter(model_file_name);
                serializer.Serialize(writer, this.model);
                writer.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when exporting svm model: " + ex.Message);
            }
        }

        /// <summary>
        /// Import the model from an xml file
        /// </summary>
        public void Import(string model_file_name)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(svm_model));
                FileStream fs = new FileStream(model_file_name, FileMode.Open);
                this.model = (svm_model) serializer.Deserialize(fs);
                fs.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when importing svm model: " + ex.Message);
            }
        }

        /// <summary>
        /// Serialize the model into an XML.
        /// </summary>
        /// <returns>The serialized model</returns>
        /// <exception cref="Exception">
        /// No trained svm model
        /// or
        /// An error occured when serializing svm model:  + ex.Message
        /// </exception>
        public string ToXML()
        {
            if (this.model == null)
                throw new Exception("No trained svm model");

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(svm_model));

                using (StringWriter sw = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sw))
                    {
                        serializer.Serialize(writer, this.model);
                        return sw.ToString(); 
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when serializing svm model: " + ex.Message);
            }
        }
        
        /// <summary>
        /// Loads the model from an XML.
        /// </summary>
        /// <param name="xml">The XML string.</param>
        /// <exception cref="Exception">An error occured when deserializing svm model:  + ex.Message</exception>
        public void LoadXML(string xml)
        {
            try
            {
                var deserializer = new XmlSerializer(typeof(svm_model));

                using (StringReader sr = new System.IO.StringReader(xml))
                {
                    using (XmlReader reader = XmlReader.Create(sr))
                    {
                        this.model = (svm_model)deserializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when deserializing svm model: " + ex.Message);
            }
        }
        
        /// <summary>
        /// Provides the prediction
        /// </summary>
        public abstract double Predict(svm_node[] x);
    }
}
